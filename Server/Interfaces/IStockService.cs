using Server.Models;

namespace Server.Interfaces;

public interface IStockService
{
    Task<IEnumerable<TwseStockInfo>> FetchStocksInfo(IEnumerable<string> stocks);
    
}