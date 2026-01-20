using Server.Models;
using Server.Services;

namespace Server.Interfaces;

public interface IUserService
{
    public Task<RegisterResult> RegisterAccount(AccountModel account);

    public Task SetStockNotifications(string account, UserStockNotificationSetting notificationSettings);
    Task<LoginResult> Login(AccountModel accountModel);
    Task<IEnumerable<string>> GetUserList();
    Task<IEnumerable<string>> GetStockList();
    Task InitialzeTelegramSetting(string? account, string telegramUsername);
    Task<UserStockNotificationSetting> GetStockNotifications(string account);
    Task<Profile> GetProfile(string account);
}