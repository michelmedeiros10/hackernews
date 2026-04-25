using HackerNewsService.Models;
using HackerNewsService.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HackerNewsService.Controllers
{
    [ApiController]
    [Route("v1/hackernews")]
    public class HackerNewsController (
        IStoriesService storiesService, 
        ILogger<HackerNewsController> logger) 
        : ControllerBase
    {
        [HttpGet("beststories")]
        public async Task<IEnumerable<StoryModel>> Get([FromQuery] string? nBest = "")
        {
            var bestStories = await storiesService.GetBestStories();

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
            return storyRank.OrderByDescending(s => s.score).ToList();
        }
    }
}
