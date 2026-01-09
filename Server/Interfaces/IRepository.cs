using Google.Cloud.Firestore;

namespace Server.Interfaces;

public interface IRepository
{
    public FirestoreDb GetDB();


}