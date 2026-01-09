using Google.Cloud.Firestore;

namespace Server.Models;

[FirestoreData]
public class StockNotificationSettingsModel
{
    [FirestoreProperty]
    public IEnumerable<StockNotificationSetting> StockNotificationSettings { get; set; }
}