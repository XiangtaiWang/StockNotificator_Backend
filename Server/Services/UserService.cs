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
    public async Task RegisterAccount(AccountModel account)
    {
        var exist = await _userRepository.CheckExist(account.Account);
        if (exist)
        {
            throw new Exception("account already exist");
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
    }

    public async Task SetStockNotifications(string account, StockNotificationSettingsModel notificationSettings)
    {
        await _userRepository.OverwriteStockNotifications(account, notificationSettings);
    }

    public async Task<string> Login(AccountModel account)
    {
        var accountDetail = await _userRepository.GetAccountDetail(account.Account);
        if (VerifyPassword(account.Password, accountDetail.EncryptedPassword, accountDetail.Salt))
        {
            return GenerateJwtToken(account.Account);
        }
        
        throw new Exception("Password incorrect");
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
        var chatId = await _messageService.FindChatId(telegramUsername);
        await _userRepository.UpdateTelegramSetting(account, telegramUsername, chatId);
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
}
