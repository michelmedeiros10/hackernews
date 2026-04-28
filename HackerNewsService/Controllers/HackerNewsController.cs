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
            int.TryParse(nBest ?? "", out int rank);
            if (rank <= 0) rank = 3; //default rank is best 3

            //Setting the cache lifetime in seconds (default is 300 seconds or 5 minutes)
            //storiesService.SetCacheLifetime(30);

			var bestStories = await storiesService.GetBestStories(rank);

            return bestStories;
        }
    }
}
