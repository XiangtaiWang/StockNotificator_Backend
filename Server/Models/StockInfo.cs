using Google.Cloud.Firestore;

namespace Server.Models;

[FirestoreData]
public class StockInfo
{
    [FirestoreProperty]
    public DateTime UpdateTime { get; set; }

    [FirestoreProperty]
    public IEnumerable<Stock> Stocks{ get; set; }

    public IDictionary<string, Stock> StockDict;

}