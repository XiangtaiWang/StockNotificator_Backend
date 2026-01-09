using Microsoft.IdentityModel.Tokens;
using Server.Interfaces;
namespace Server.Controllers;

public class JobService 
{
    private IUserService _userService;
    private IDataCenterService _dataCenterService;
    private IStockNotificationService _stockNotificationService;

    public JobService(IUserService userService, IDataCenterService dataCenterService, IStockNotificationService stockNotificationService)
    {
        _userService = userService;
        _dataCenterService = dataCenterService;
        _stockNotificationService = stockNotificationService;
    }


    public async Task SystemMinutelyJob()
    {
        Console.Write("running SystemMinutelyJob");
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
            Console.WriteLine("No User need to be notified");
        }
        await _stockNotificationService.Notify(userList);
        
    }
}