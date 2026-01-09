using Google.Cloud.Firestore;

namespace Server.Models;

[FirestoreData]
public class RegisterAccountModel
{
    [FirestoreProperty]
    public string Account { get; set; }

    [FirestoreProperty]
    public string EncryptedPassword { get; set; }
    
    [FirestoreProperty]
    public string Salt { get; set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; set; }
    
    [FirestoreProperty]
    public string TelegramUsername { get; set; }
    
    [FirestoreProperty]
    public long TelegramChatId { get; set; }
}