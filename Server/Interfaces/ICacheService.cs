namespace Server.Interfaces;

public interface ICacheService
{
    void Write(string key, string value);
    public string Read(string key);
    bool Exist(string key);
}