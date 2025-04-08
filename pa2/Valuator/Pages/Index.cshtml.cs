using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _redis;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public void OnGet()
    {

    }

    private double GetRank(string text)
    {
        double notLetters = 0;

        foreach (char ch in text)
            if (!char.IsLetter(ch))
                notLetters++;

        return notLetters / text.Length;
    }

    private double GetSimilarity(IDatabase db, string text, string textKey)
    {
        //var keys = _redis.GetServer(_redis.GetEndPoints().First()).Keys(pattern: "TEXT-:*");
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

    public IActionResult OnPost(string text)
    {

        if (string.IsNullOrWhiteSpace(text))
        {
            return Redirect("index");
        }

        _logger.LogDebug(text);
        _logger.LogInformation(text);

        var db = _redis.GetDatabase();

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        // TODO: (pa1) сохранить в БД (Redis) text по ключу textKey
        db.StringSet(textKey, text);

        string rankKey = "RANK-" + id;
        // TODO: (pa1) посчитать rank и сохранить в БД (Redis) по ключу rankKey
        db.StringSet(rankKey, GetRank(text));

        string similarityKey = "SIMILARITY-" + id;
        // TODO: (pa1) посчитать similarity и сохранить в БД (Redis) по ключу similarityKey
        db.StringSet(similarityKey, GetSimilarity(db, text, textKey));

        return Redirect($"summary?id={id}");
    }
}
