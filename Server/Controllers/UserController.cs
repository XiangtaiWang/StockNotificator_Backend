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
        await _userService.RegisterAccount(accountModel);
        
        return new OkResult();
    }
    [HttpPost("Login")]
    public async Task<ActionResult> Login(AccountModel loginModel)
    {
        var token = await _userService.Login(loginModel);

        return Ok(token);
    }
    
    [Authorize]
    [HttpPost("SetStockNotifications")]
    public async Task<ActionResult> SetStockNotifications(StockNotificationSettingsModel notificationSettings)
    {
        var account = User.FindFirst("Account")?.Value;
        await _userService.SetStockNotifications(account, notificationSettings);

        return new OkResult();
    }
    
    [Authorize]
    [HttpPost("SetTelegram")]
    public async Task<ActionResult> SetStockNotifications(string telegramUsername)
    {
        var account = User.FindFirst("Account")?.Value;
        
        await _userService.InitialzeTelegramSetting(account, telegramUsername);
        var chatId = await _dataCenterService.GetChatId(account);
        await _messageService.SendMessage(chatId, $"Hi {telegramUsername} \nTelegram Setting success");

        return new OkResult();
    }
}