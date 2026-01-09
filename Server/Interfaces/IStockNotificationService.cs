namespace Server.Interfaces;

public interface IStockNotificationService
{
    Task Notify(IEnumerable<string> userList);
}