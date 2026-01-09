using System.Text.Json.Serialization;

namespace Server.Models;

public class TwseStockInfo
{
    [JsonPropertyName("c")]
    public string? Code { get; set; }             // 股票代碼

    [JsonPropertyName("n")]
    public string? Name { get; set; }             // 股票名稱

    [JsonPropertyName("z")]
    public string? Price { get; set; }            // 最新成交價



}