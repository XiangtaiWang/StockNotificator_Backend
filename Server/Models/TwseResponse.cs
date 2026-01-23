using System.Text.Json.Serialization;
using Server.Services;

namespace Server.Models;

public class TwseResponse
{
    [JsonPropertyName("msgArray")]
    public IEnumerable<TwseStockInfo>? MsgArray { get; set; }
    
}