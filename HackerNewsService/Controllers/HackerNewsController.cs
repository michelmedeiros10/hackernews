using HackerNewsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsService.Controllers
{
    [ApiController]
    [Route("v1/hackernews")]
    public class HackerNewsController : ControllerBase
    {
        private readonly ILogger<HackerNewsController> _logger;

        public HackerNewsController(ILogger<HackerNewsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("beststories")]
        public IEnumerable<StoryModel> Get([FromQuery] string? nBest = "")
        {
            int.TryParse(nBest ?? "", out int rank);

            if (rank == 0) rank = 5; //default rank
            List<StoryModel> storyRank = [];
            for (int i = 1; i <= rank; i++)
            {
                storyRank.Add(new StoryModel 
                {
                    by = $"Author{i}",
                    id = i,
                    title = $"Title{i}",
                    score = i * Random.Shared.Next(10,15),
                    time = DateTime.UtcNow.Ticks
                });
            }
            return storyRank.OrderBy(s => s.score).ToList();
        }
    }
}
