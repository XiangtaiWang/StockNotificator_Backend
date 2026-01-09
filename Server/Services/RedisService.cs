using Server.Interfaces;
using StackExchange.Redis;

namespace Server.Services;

public class RedisService : ICacheService
{
    private IDatabase _database;

    public RedisService(IConnectionMultiplexer multiplexer)
    {
        _database = multiplexer.GetDatabase(0);
    }

    public void Write(string key, string value)
    { 
        _database.StringSetAsync(key, value);
    }

    public string Read(string key)
    {
        var redisValues = _database.StringGet(key);
        return redisValues;
    }

    public bool Exist(string key)
    {
        return _database.KeyExists(key);
    }
}