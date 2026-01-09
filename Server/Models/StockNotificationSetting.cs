using Google.Cloud.Firestore;

namespace Server.Models;

[FirestoreData]
public class StockNotificationSetting
{
    [FirestoreProperty]
    public string StockCode{ get; set; }
    [FirestoreProperty]
    public int IntervalMinute { get; set; }
}