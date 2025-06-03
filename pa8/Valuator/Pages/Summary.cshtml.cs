using DBController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    public IActionResult OnGet(string id)
    {
        _logger.LogDebug(id);

        string usernameActual = User.Identity.Name;
        if (string.IsNullOrWhiteSpace(usernameActual))
            return Redirect("login");

        string shard = _redis.Get(id, "MAIN");
        string value1 = _redis.Get($"RANK-{id}", shard);
        string value2 = _redis.Get($"SIMILARITY-{id}", shard);
        string value3 = _redis.Get(id, shard);

        if (usernameActual != value3)
            return Redirect("index");

        if (double.TryParse(value1, out double rank))
            Rank = rank;

        if (double.TryParse(value2, out double similarity))
            Similarity = similarity;

        return Page();
    }
}
