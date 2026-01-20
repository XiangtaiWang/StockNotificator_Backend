using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Server.Interfaces;
using Server.Models;

namespace Server.Services;

public class UserService : IUserService
{
    private IUserRepository _userRepository;
    private IConfiguration _configuration;
    private IMessageService _messageService;

    public UserService(IUserRepository userRepository, IConfiguration configuration, IMessageService messageService)
    {
        _configuration = configuration;
        _messageService = messageService;
        _userRepository = userRepository;
    }
    public async Task<RegisterResult> RegisterAccount(AccountModel account)
    {
        var exist = await _userRepository.CheckExist(account.Account);
        if (exist)
        {
            return new RegisterResult()
            {
                IsSuccessful = false,
                Message = "Account already exist"
            };
        }

        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToBase64String(saltBytes);
        var encryptedPassword = ComputeHash(account.Password, salt);

        var registerAccountModel = new RegisterAccountModel()
        {
            Account = account.Account,
            EncryptedPassword = encryptedPassword,
            Salt = salt
        };
        
        await _userRepository.AddUser(registerAccountModel);
        return new RegisterResult()
        {
            IsSuccessful = true
        };
    }

    public async Task SetStockNotifications(string account, UserStockNotificationSetting notificationSettings)
    {
        await _userRepository.OverwriteStockNotifications(account, notificationSettings);
    }

    public async Task<LoginResult> Login(AccountModel account)
    {
        var accountDetail = await _userRepository.GetAccountDetail(account.Account);
        if (accountDetail!=null)
        {
            if (VerifyPassword(account.Password, accountDetail.EncryptedPassword, accountDetail.Salt))
            {
                return new LoginResult()
                { 
                    IsSuccessful = true,
                    Token = GenerateJwtToken(account.Account)
                };
            }

            return new LoginResult()
            {
                IsSuccessful = false
            };
        }

        return new LoginResult()
        {
            IsSuccessful = false
        };
    }

    public async Task<IEnumerable<string>> GetUserList()
    {
        return await _userRepository.GetAllUser();
    }

    public async Task<IEnumerable<string>> GetStockList()
    {
        return await _userRepository.GetAllStock();

    }

    public async Task InitialzeTelegramSetting(string? account, string telegramUsername)
    {
        if (telegramUsername.StartsWith("@"))
        {
            telegramUsername = telegramUsername.Substring(1);
        }

        
        var chatId = await _messageService.FindChatId(telegramUsername);
        await _userRepository.UpdateTelegramSetting(account, telegramUsername, chatId);
    }

    public async Task<UserStockNotificationSetting> GetStockNotifications(string account)
    {
        return await _userRepository.GetUserNotificationSettings(account);
    }


    private string ComputeHash(string password, string salt)
    {
 
        var combined = password + salt;
        var inputBytes = Encoding.UTF8.GetBytes(combined);
        var hashBytes = SHA256.HashData(inputBytes);

        return Convert.ToBase64String(hashBytes);
    }

    private bool VerifyPassword(string inputPassword, string storedPassword, string storedSalt)
    {
        var currentHash = ComputeHash(inputPassword, storedSalt);
        return storedPassword == currentHash;
    }
    private string GenerateJwtToken(string username)
    {
        var claims = new List<Claim> {
            new Claim("Account", username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1), // 有效期一天
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
    
    public async Task<Profile> GetProfile(string account)
    {
        var accountDetail = await _userRepository.GetAccountDetail(account);
        return new Profile()
        {
            TelegramUserName = accountDetail.TelegramUsername,
            TelegramHasBeenSet = accountDetail.TelegramChatId!=0
            
        };

    }
    
    
}

public class LoginResult
{
    public bool IsSuccessful { get; set; }
    public string Token { get; set; }
}
