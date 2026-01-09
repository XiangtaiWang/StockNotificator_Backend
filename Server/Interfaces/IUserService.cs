using Server.Models;

namespace Server.Interfaces;

public interface IUserService
{
    public Task RegisterAccount(AccountModel account);

    public Task SetStockNotifications(string account, StockNotificationSettingsModel notificationSettings);
    Task<string> Login(AccountModel accountModel);
    Task<IEnumerable<string>> GetUserList();
    Task<IEnumerable<string>> GetStockList();
    Task InitialzeTelegramSetting(string? account, string telegramUsername);
}