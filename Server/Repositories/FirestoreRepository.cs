using Google.Api.Gax;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Options;
using Server.Interfaces;

namespace Server.Repositories;

public class FirestoreRepository:IRepository
{
    private FirestoreDb Db { get; }

    public FirestoreRepository(IOptions<FirebaseSetting> option)
    {
        
        var firebaseSetting = option.Value;

        var dbBuilder = new FirestoreDbBuilder
        {
            ProjectId = firebaseSetting.ProjectId,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction

        };
        if (!string.IsNullOrEmpty(firebaseSetting.EmulatorHost))
        {
            dbBuilder.Endpoint = firebaseSetting.EmulatorHost;
        }

        Db = dbBuilder.Build();
    }

    public FirestoreDb GetDB()
    {
        return Db;
    }
    
}