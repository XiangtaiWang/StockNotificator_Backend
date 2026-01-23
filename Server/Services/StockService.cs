using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Server.Interfaces;
using Server.Models;


namespace Server.Services;

public class StockService : IStockService
{
    private const string Url = "https://mis.twse.com.tw/stock/api/getStockInfo.jsp?ex_ch=";
    private HttpClient client;
    private ILogService _logService;

    public StockService(HttpClient client, ILogService logService)
    {
        _logService = logService;
        this.client = client;
    }


    public async Task<IEnumerable<TwseStockInfo>> FetchStocksInfo(IEnumerable<string> stocks)
    {
        if (stocks.IsNullOrEmpty())
        {
            return new List<TwseStockInfo>();
        }
        
        var url = $"{Url}{GenerateRequestString(stocks)}";
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            var log = new LogModel()
            {
                Level = LogLevel.Error,
                Message = $"FetchStocksInfo :{string.Join(", ", stocks)}\nStatus Code: {response.StatusCode}\n Message: {errorBody}",
                Timestamp = DateTime.UtcNow
            };
            await _logService.Write(log);
            return new List<TwseStockInfo>();
        }
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<TwseResponse>(content);
        
        return !data.MsgArray.IsNullOrEmpty()?data.MsgArray:ImmutableList<TwseStockInfo>.Empty;
    }

    private string GenerateRequestString(IEnumerable<string> stockCodes)
    {
        var result = new StringBuilder();
        for (var i = 0; i < stockCodes.Count(); i++)
        {
            result.Append("tse_");
            result.Append(stockCodes.ElementAt(i));
            result.Append(".tw");

            if (i<stockCodes.Count()-1)
            {
                result.Append('|');
            }
        }

        return result.ToString();
    }

}
