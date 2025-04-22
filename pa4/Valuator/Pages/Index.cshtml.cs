using System.Threading.Tasks;
using MessageBroker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly IMessageBroker _messageBroker;
    string rankExchangeName = "valuator.processing.rank";
    string similarityExchangeName = "events.loger";//!!!
    string similarityRoutingKey = "SimilarityCalculated";//!!!

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis, IMessageBroker messageBroker)
    {
        _logger = logger;
        _redis = redis;
        _messageBroker = messageBroker;
    }

    public void OnGet(){}

    private double GetSimilarity(IDatabase db, string text, string textKey)
    {
        IServer server = _redis.GetServer(_redis.GetEndPoints().First());
        IEnumerable<RedisKey> keys = server.Keys(pattern: "TEXT-*");

        foreach (var key in keys)
        {
            if (key == textKey || text != db.StringGet(key))
                continue;
            return 1;
        }
        return 0;
    }

    public async Task<IActionResult> OnPost(string text)
    {

        if (string.IsNullOrWhiteSpace(text))
        {
            return Redirect("index");
        }

        //_logger.LogDebug(text);
        //_logger.LogInformation(text);

        var db = _redis.GetDatabase();

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        db.StringSet(textKey, text);

        string similarityKey = "SIMILARITY-" + id;
        double similarity = GetSimilarity(db, text, textKey);
        db.StringSet(similarityKey, similarity);

        await _messageBroker.SendMessageAsync(similarityExchangeName, $"id: {id}, similarity: {similarity}", similarityRoutingKey);//!!!

        await _messageBroker.SendMessageAsync(rankExchangeName, id);

        return Redirect($"summary?id={id}");
    }
}
