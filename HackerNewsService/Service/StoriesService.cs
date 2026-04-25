using System.Net.Http.Json;

namespace HackerNewsService.Service;

public class StoriesService (
	ILogger<StoriesService> logger,
	HttpClient httpClient) 
	: IStoriesService
{
	public async Task<List<int>> GetBestStories()
	{
		var bestStories = new List<int>();

		try
		{
			bestStories = await httpClient.GetFromJsonAsync<List<int>>("v0/beststories.json");
			bestStories ??= [];
			bestStories = bestStories.OrderBy(a => a).ToList();
		}
		catch (Exception ex)
		{
			//TODO
		}

		return bestStories;
	}

}
