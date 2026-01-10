using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Interfaces;
namespace Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class JobController 
{
    private IUserService _userService;
    private IDataCenterService _dataCenterService;
    private IStockNotificationService _stockNotificationService;

    public JobController(IUserService userService, IDataCenterService dataCenterService, IStockNotificationService stockNotificationService)
    {
        _userService = userService;
        _dataCenterService = dataCenterService;
        _stockNotificationService = stockNotificationService;
    }

    [HttpPost("SystemMinutelyJob")]
    public async Task SystemMinutelyJob()
    {
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
            // Console.WriteLine("No User need to be notified");
            return;
        }
        await _stockNotificationService.Notify(userList);
        
    }
}