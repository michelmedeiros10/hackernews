namespace HackerNewsService.Service;

public interface IStoriesService
{
	Task<List<int>> GetBestStories();
}
