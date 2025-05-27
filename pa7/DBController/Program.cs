using StackExchange.Redis;

namespace DBController;
public interface IDBController
{
    string Get(string shardKey, string key);

    void Set(string shardKey, string key, string value);

    IServer GetServerOfDB(string shardKey);
}


public class RedisDB : IDBController
{
    public string Get(string key, string shard)
    {
        IDatabase db = ConnectToDB(shard);
        Console.WriteLine($"LOOKUP: key: {key}, shard: {shard}");
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
        string connectionStr;        
        switch (shard)
        {
            case "MAIN":
                connectionStr = "redismain:6379";
                break;
            case "RU":
                connectionStr = "redisru:6379";
                break;
            case "EU":
                connectionStr = "rediseu:6379";
                break;
            case "ASIA":
                connectionStr = "redisasia:6379";
                break;
            default:
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