using StackExchange.Redis;

namespace DBController;

public class RedisDB
{
    public string Get(string key, string shard)
    {
        IDatabase db = ConnectToDB(shard);
        Console.WriteLine($"key: {key}, shard: {shard}");
        return db.StringGet(key);
    }

    public void Set(string shard, string key, string value)
    {
        IDatabase db = ConnectToDB(shard);
        db.StringSet(key, value);
    }

    private IDatabase ConnectToDB(string shard)
    {
        return GetConnectionMultiplexer(shard).GetDatabase();
    }

    private IConnectionMultiplexer GetConnectionMultiplexer(string shard)
    {
        string environmentLocation = $"REDIS_{shard}_CONNECTION_STR";
        string connectionStr = Environment.GetEnvironmentVariable(environmentLocation);

        if (string.IsNullOrWhiteSpace(connectionStr))
        {
            throw new ArgumentException("Unknown shard");
        }

        return ConnectionMultiplexer.Connect(connectionStr);
    }

    public IServer GetServerOfDB(string shard)
    {
        IConnectionMultiplexer connectionMultiplexer = GetConnectionMultiplexer(shard);
        return connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
    }
};