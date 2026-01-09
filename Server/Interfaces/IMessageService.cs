namespace Server.Interfaces;

public interface IMessageService
{
    Task<bool> SendMessage(long chatid, string content);
    public Task<long> FindChatId(string targetUsername);
}