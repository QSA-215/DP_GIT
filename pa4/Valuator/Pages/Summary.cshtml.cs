using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IConnectionMultiplexer _redis;

    public SummaryModel(ILogger<SummaryModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        // TODO: (pa1) проинициализировать свойства Rank и Similarity значениями из БД (Redis)

        var db = _redis.GetDatabase();
        while (db.StringGet($"RANK-{id}") == RedisValue.Null)
        {
            Rank = (double)db.StringGet($"RANK-{id}");
        }
        Rank = (double)db.StringGet($"RANK-{id}");
        Similarity = (double)db.StringGet($"SIMILARITY-{id}");
    }
}
