using System.Threading.Tasks;
using MessageBroker;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace RankCalculator;

public class RankCalculatorService
{
    private readonly IDatabase _db;
    private readonly IMessageBroker _messageBroker;
    string rankExchangeName;
    string rankRoutingKey;


    public RankCalculatorService(IDatabase db, IMessageBroker messageBroker)
    {
        _db = db;
        rankExchangeName = "events.loger";
        rankRoutingKey = "RankCalculated";
        _messageBroker = messageBroker;
    }

    public async void Proccess(string id)
    {
        string text = _db.StringGet($"TEXT-{id}");

        double rank = CalculateRank(text);
        Console.WriteLine($"{id} has rank: {rank}");

        await SendRankAsync(id, rank);

        _db.StringSet($"RANK-{id}", rank);
    }

    private double CalculateRank(string text)
    {
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
