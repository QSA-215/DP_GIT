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

        string password;
        switch (shard)
        {
            case "MAIN":
                password = "my_redis_main_password";
                break;
            case "RU":
                password = "my_redis_ru_password";
                break;
            case "EU":
                password = "my_redis_eu_password";
                break;
            case "ASIA":
                password = "my_redis_asia_password";
                break;
            case "USERS":
                password = "my_redis_users_password";
                break;
            default:
                throw new ArgumentException("Unknown shard");
        }

        ConfigurationOptions options = ConfigurationOptions.Parse(connectionStr);
        options.Password = password;
        return ConnectionMultiplexer.Connect(options);
    }

    public IServer GetServerOfDB(string shard)
    {
        IConnectionMultiplexer connectionMultiplexer = GetConnectionMultiplexer(shard);
        return connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());
    }
};