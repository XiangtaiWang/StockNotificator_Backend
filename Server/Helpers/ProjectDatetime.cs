namespace Server.Helpers;

public static class ProjectDatetime
{
    private const string TaipeiStandardTime = "Taipei Standard Time";

    public static DateTime DateTimeNow()
    {
        var taiwanZone = TimeZoneInfo.FindSystemTimeZoneById(TaipeiStandardTime);
        var utcNow = DateTime.UtcNow;
        var taiwanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, taiwanZone);
        
        return taiwanTime;
    }
    
}