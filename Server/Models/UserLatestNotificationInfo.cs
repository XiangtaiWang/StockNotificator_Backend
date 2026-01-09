namespace Server.Models;

public class UserLatestNotificationInfo
{
    public string User { get; set; }

    public IDictionary<string, DateTime> LatestNotificationInfos{ get; set; }

}