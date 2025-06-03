using DBController;
using MessageBroker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly RedisDB _redis;
    private readonly IMessageBroker _messageBroker;
    string rankExchangeName = "valuator.processing.rank";
    string similarityExchangeName = "events.loger";
    string similarityRoutingKey = "SimilarityCalculated";

    public IndexModel(ILogger<IndexModel> logger, RedisDB redis, IMessageBroker messageBroker)
    {
        _logger = logger;
        _redis = redis;
        _messageBroker = messageBroker;
    }

    public void OnGet(){}

    private double GetSimilarity(string text, string textKey, string shard)
    {
        IServer server = _redis.GetServerOfDB(shard);

        string[] keys = server.Keys(pattern: "TEXT-*").Select(k => (string)k).ToArray();
        string[] values = keys.Select(key => _redis.Get(key, shard)).ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            if (string.Compare(keys[i], textKey) == 0)
                continue;

            if (string.Compare(text, values[i]) == 0)
                return 1;
        }
        return 0;
    }

    public async Task<IActionResult> OnPost(string text, string region)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Redirect("index");
        }

        string username = User.Identity.Name;

        if (string.IsNullOrWhiteSpace(username))
        {
            return Redirect("login");
        }

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;

        string similarityKey = "SIMILARITY-" + id;
        double similarity = GetSimilarity(text, textKey, region);

        _redis.Set("MAIN", id, region);
        _redis.Set(region, id, username);
        _redis.Set(region, textKey, text);
        _redis.Set(region, similarityKey, similarity.ToString());

        await _messageBroker.SendMessageAsync(similarityExchangeName, $"id: {id}, similarity: {similarity}", similarityRoutingKey);

        await _messageBroker.SendMessageAsync(rankExchangeName, id);

        return Redirect($"summary?id={id}");
    }
}
