using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Interfaces;
using Server.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase 
{
    private IUserService _userService;
    private IMessageService _messageService;
    private IDataCenterService _dataCenterService;

    public UserController(IUserService userService, IMessageService messageService, IDataCenterService dataCenterService)
    {
        _userService = userService;
        _messageService = messageService;
        _dataCenterService = dataCenterService;
    }
    
    [HttpPost("RegisterAccount")]
    public async Task<ActionResult> RegisterAccount(AccountModel accountModel)
    {
        var result = await _userService.RegisterAccount(accountModel);

        return Ok(result);
    }
    [HttpPost("Login")]
    public async Task<ActionResult> Login(AccountModel loginModel)
    {
        var result = await _userService.Login(loginModel);

        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("SetStockNotifications")]
    public async Task<ActionResult> SetStockNotifications(UserStockNotificationSetting notificationSettings)
    {
        var account = User.FindFirst("Account")?.Value;
        await _userService.SetStockNotifications(account, notificationSettings);
        await _dataCenterService.UpdateUserNotificationSettingsCache(account);
        return new OkResult();
    }
    
    [Authorize]
    [HttpPost("SetTelegram")]
    public async Task<ActionResult> SetTelegram(SetTelegramRequest request)
    {
        var telegramUsername = request.TelegramUsername;
        var account = User.FindFirst("Account")?.Value;
        
        await _userService.InitialzeTelegramSetting(account, telegramUsername);
        var chatId = await _dataCenterService.GetChatId(account);
        await _messageService.SendMessage(chatId, $"Hi {telegramUsername} \nTelegram Setting success");

        return new OkResult();
    }
    [Authorize]
    [HttpPost("GetTelegram")]
    public async Task<Profile> GetTelegram()
    {
        var account = User.FindFirst("Account")?.Value;
        
        return await _userService.GetProfile(account);
    }
    
    [Authorize]
    [HttpPost("GetStockNotifications")]
    public async Task<UserStockNotificationSetting> GetStockNotifications()
    {
        var account = User.FindFirst("Account")?.Value;

        return await _userService.GetStockNotifications(account);
    }
    
}