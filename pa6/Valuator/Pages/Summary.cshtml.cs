//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using DBController;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly RedisDB _redis;

    public SummaryModel(ILogger<SummaryModel> logger, RedisDB redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        string shard = _redis.Get(id, "MAIN");

        Console.WriteLine("sharddd", shard);

        while (_redis.Get($"RANK-{id}", shard) == RedisValue.Null)
        {
            double.Parse(_redis.Get($"RANK-{id}", shard));
        }
        //Rank = double.Parse(_redis.Get($"RANK-{id}", shard));
        Similarity = double.Parse(_redis.Get($"SIMILARITY-{id}", shard));
    }
}
