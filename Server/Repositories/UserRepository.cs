using Google.Cloud.Firestore;
using Server.Interfaces;
using Server.Models;

namespace Server.Repositories;

public class UserRepository : IUserRepository
{
    private FirestoreDb _Db;
    private string _usernotificationsettingKey = "UserNotificationSetting";

    public UserRepository(IRepository repository)
    {
        _Db = repository.GetDB();
    }
    
    public async Task AddUser(RegisterAccountModel accountModel)
    {
        var docRef = _Db.Collection("Users").Document(accountModel.Account);
        accountModel.CreatedAt = DateTime.UtcNow;
        await docRef.SetAsync(accountModel);
    }

    public async Task<bool> CheckExist(string account)
    {
        
        var docRef = _Db.Collection("Users").Document(account);

        var snapshot = await docRef.GetSnapshotAsync();

        return snapshot.Exists;
    }

    public async Task OverwriteStockNotifications(string account, UserStockNotificationSetting notificationSettings)
    {
        var docRef = _Db.Collection(_usernotificationsettingKey).Document(account);
        await docRef.SetAsync(notificationSettings);
    }

    public async Task<RegisterAccountModel?> GetAccountDetail(string account)
    {
        var docRef = _Db.Collection("Users").Document(account);

        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<RegisterAccountModel>();
        }

        return null;
    }

    public async Task<IEnumerable<string>> GetAllUser()
    {
        var docRef = _Db.Collection("Users");
        var snapshot = await docRef.GetSnapshotAsync();
        var accounts = snapshot.Documents.Select(doc => doc.Id);
        return accounts;
    }

    public async Task<IDictionary<string, long>> GetUserChatIdDict()
    {
        var docRef = _Db.Collection("Users");
        var snapshot = await docRef.GetSnapshotAsync();
        
        var userList = snapshot.Documents
            .Select(doc => doc.ConvertTo<RegisterAccountModel>())
            .ToList();

        return userList.ToDictionary(user => user.Account, user => user.TelegramChatId);
    }

    public async Task UpdateTelegramSetting(string account, string telegramUsername, long chatId)
    {
        var docRef = _Db.Collection("Users").Document(account);
        
        var updates = new Dictionary<string, object>
        {
            { nameof(RegisterAccountModel.TelegramChatId), chatId },
            { nameof(RegisterAccountModel.TelegramUsername), telegramUsername },
        };
        
        await docRef.UpdateAsync(updates);
    }

    public async Task<UserStockNotificationSetting> GetUserNotificationSettings(string account)
    {
        var docRef = _Db.Collection(_usernotificationsettingKey).Document(account);
        var snapshotAsync = await docRef.GetSnapshotAsync();

        return snapshotAsync.ConvertTo<UserStockNotificationSetting>();
    }

    public async Task<HashSet<string>> GetAllStock()
    {
        var docRef = _Db.Collection(_usernotificationsettingKey);
        var snapshotAsync = await docRef.GetSnapshotAsync();
        var stocks = new HashSet<string>();
        foreach (var document in snapshotAsync.Documents)
        {
            var userStockNotificationSetting = document.ConvertTo<UserStockNotificationSetting>();

            stocks.UnionWith(userStockNotificationSetting.StockNotificationSettings.Select(s=>s.StockCode));
        }

        return stocks;
    }
}