using Google.Cloud.Firestore;

namespace Server.Models;

[FirestoreData]
public class LogModel
{
    [FirestoreProperty]
    public DateTime Timestamp { get; set; }
    [FirestoreProperty]
    public LogLevel Level { get; set; }
    [FirestoreProperty]
    public string Message { get; set; }
}