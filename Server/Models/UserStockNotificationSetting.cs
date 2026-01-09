using Google.Cloud.Firestore;

namespace Server.Models;

[FirestoreData]
public class UserStockNotificationSetting
{
    
    // public string User;
    [FirestoreProperty]
    public IEnumerable<StockNotificationSetting> StockNotificationSettings { get; set; }
}