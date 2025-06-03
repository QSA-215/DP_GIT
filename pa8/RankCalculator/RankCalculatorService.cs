using MessageBroker;
using DBController;

namespace RankCalculator;

public class RankCalculatorService
{
    private readonly IDBController _db;
    private readonly IMessageBroker _messageBroker;
    string rankExchangeName;
    string rankRoutingKey;


    public RankCalculatorService(IDBController db, IMessageBroker messageBroker)
    {
        _db = db;
        rankExchangeName = "events.loger";
        rankRoutingKey = "RankCalculated";
        _messageBroker = messageBroker;
    }

    public async Task Proccess(string id)
    {
        string shard = _db.Get(id, "MAIN");//!!!
        string text = _db.Get($"TEXT-{id}", shard);//!!!

        double rank = CalculateRank(text);
        Console.WriteLine($"{id} has rank: {rank}");

        await SendRankAsync(id, rank);

        _db.Set(shard, $"RANK-{id}", rank.ToString());
    }

    private double CalculateRank(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        double count = 0;

        foreach (char ch in text)
        {
            if (!char.IsLetter(ch))
            {
                count++;
            }
        }

        return count / text.Length;
    }
    private async Task SendRankAsync(string id, double rank)
    {
        await _messageBroker.SendMessageAsync(rankExchangeName, $"id: {id}, rank: {rank}", rankRoutingKey);
    }
}
