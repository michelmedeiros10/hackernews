using HackerNewsService.Models;

namespace HackerNewsService.Service;

public interface IStoriesService
{
	Task<List<int>> GetBestStoriesIds();
	Task<List<StoryModel>> GetBestStories(int num);
	void SetCacheLifetime(int seconds);
}
