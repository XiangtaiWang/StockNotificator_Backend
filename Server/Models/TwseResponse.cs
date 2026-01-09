using System.Text.Json.Serialization;
using Server.Services;

namespace Server.Models;

public class TwseResponse
{
    [JsonPropertyName("msgArray")]
    public IEnumerable<TwseStockInfo>? MsgArray { get; set; }

    // [JsonPropertyName("rtcode")]
    // public string? RtCode { get; set; }
    //
    // [JsonPropertyName("rtmessage")]
    // public string? RtMessage { get; set; }
    //
    // [JsonPropertyName("queryTime")]
    // public TwseQueryTime? QueryTime { get; set; }
}