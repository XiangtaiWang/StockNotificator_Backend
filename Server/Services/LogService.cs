using Google.Cloud.Firestore;
using Server.Interfaces;
using Server.Models;

namespace Server.Services;

public class LogService : ILogService
{
    private FirestoreDb _Db;

    public LogService(IRepository repository)
    {
        _Db = repository.GetDB();
    }
    public async Task Write(LogModel logModel)
    {
        var docId = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var docRef = _Db.Collection("SystemLogs").Document(docId);

        await docRef.SetAsync(new { 
            logs = FieldValue.ArrayUnion(logModel) 
        }, SetOptions.MergeAll);
    }
}