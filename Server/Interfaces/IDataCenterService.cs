using Server.Models;

namespace Server.Interfaces;

public interface IDataCenterService
{
    Task UpdateStockInfoJob();
    StockInfo? GetStockInfo();
    Task<UserStockNotificationSetting> GetUserNotificationSettings(string user);
    UserLatestNotificationInfo GetUserLatestNotification(string user);
    Task<long> GetChatId(string user);
    void UpdateUserLatestNotification(UserLatestNotificationInfo userLatestNotification);
}