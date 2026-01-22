namespace Server.Helpers;

public static class DateTimeExtensions
{
    public static DateTime ConvertToTaipeiTimeZone(this DateTime dateTime)
    {
        TimeZoneInfo taiwanZone;
        try
        {
            taiwanZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {

            taiwanZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");
        }
        
        var taiwanTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, taiwanZone);

        return taiwanTime;

    }
}