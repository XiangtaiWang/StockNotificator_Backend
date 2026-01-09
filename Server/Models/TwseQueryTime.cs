using System.Text.Json.Serialization;

namespace Server.Models;

public class TwseQueryTime
{
    [JsonPropertyName("sysTime")]
    public string? SysTime { get; set; }

    [JsonPropertyName("sessionKey")]
    public string? SessionKey { get; set; }
}