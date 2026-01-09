using Server.Models;

namespace Server.Interfaces;

public interface IUserRepository
{
    public Task AddUser(RegisterAccountModel accountModel);
    public Task<bool> CheckExist(string account);
    public Task OverwriteStockNotifications(string account, StockNotificationSettingsModel notificationSettings);
    public Task<RegisterAccountModel> GetAccountDetail(string accountAccount);
    Task<IEnumerable<string>> GetAllUser();
    Task<IDictionary<string, long>> GetUserChatIdDict();
    Task UpdateTelegramSetting(string account, string telegramUsername, long chatId);
    Task<UserStockNotificationSetting> GetUserNotificationSettings(string account);
    Task<HashSet<string>> GetAllStock();
}