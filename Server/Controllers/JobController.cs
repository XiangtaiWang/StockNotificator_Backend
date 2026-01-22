using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Helpers;
using Server.Interfaces;
using Server.Models;

namespace Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class JobController 
{
    private readonly string _expectedKey;
    private IUserService _userService;
    private IDataCenterService _dataCenterService;
    private IStockNotificationService _stockNotificationService;
    private ILogService _logService;

    public JobController(IConfiguration configuration, IUserService userService, IDataCenterService dataCenterService, IStockNotificationService stockNotificationService, ILogService logService)
    {
        _logService = logService;
        _expectedKey = configuration["SYSTEMJOB_KEY"];
        _userService = userService;
        _dataCenterService = dataCenterService;
        _stockNotificationService = stockNotificationService;
    }

    [HttpPost("SystemMinutelyJob")]
    public async Task SystemMinutelyJob([FromHeader(Name = "SYSTEMJOB_KEY")] string receivedKey)
    {
        if (string.IsNullOrEmpty(receivedKey) || receivedKey != _expectedKey)
        {

            await _logService.Write(new LogModel()
            {
                Timestamp = ProjectDatetime.DateTimeNow(),
                Message = "SystemMinutelyJob called by unknown",
                Level = LogLevel.Warning
            });
            return;
        }
        await UpdateStockInfo();
        await PushNotification();
    }
    private async Task UpdateStockInfo()
    {
        await _dataCenterService.UpdateStockInfoJob();
        
    }
    
    private async Task PushNotification()
    {
        var userList = await _userService.GetUserList();

        if (userList.IsNullOrEmpty())
        {
            return;
        }
        await _stockNotificationService.Notify(userList);
        
    }
}