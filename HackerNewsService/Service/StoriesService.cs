using HackerNewsService.Infra;
using HackerNewsService.Models;
using System.Net.Http.Json;

namespace HackerNewsService.Service;

public class StoriesService (
	ILogger<StoriesService> logger,
	HttpClient httpClient,
	ICache cache) 
	: IStoriesService
{
	public async Task<List<int>> GetBestStoriesIds()
	{
		var bestStories = cache.GetBestStoriesIds();

		if (bestStories.Count > 0)
		{
			return bestStories;
		}

		try
		{
			bestStories = await httpClient.GetFromJsonAsync<List<int>>("v0/beststories.json");
			bestStories ??= [];
			bestStories = bestStories.OrderBy(a => a).ToList();
		}
		catch (Exception ex)
		{
			bestStories = cache.GetBestStoriesIds();
		}

		cache.SetBestStoriesIds(bestStories!);

		return bestStories!;
	}

	public async Task<List<StoryModel>> GetBestStories(int num)
	{
		var bestStories = cache.GetBestStories(num);
		if (bestStories.Count > 0)
		{
			return bestStories;
		}

		//Avoiding overload on calling hackernews endpoints
		if (cache.IsUpdating())
		{
			return [];
		}

		var bestStoriesIds = await GetBestStoriesIds();

		cache.SetUpdating(true);

		foreach (var id in bestStoriesIds)
		{
			try
			{
				var story = await httpClient.GetFromJsonAsync<StoryModel>($"v0/item/{id}.json");
				if (story != null)
				{
					cache.AddStory(story);
				}
			}
			catch (Exception ex)
			{
			}
		}

		cache.SetUpdating(false);

		cache.CacheUpdated();

		bestStories = cache.GetBestStories(num);

		return bestStories;
	}

	public void SetCacheLifetime(int seconds)
	{
		cache.SetCacheLifetime(seconds);
	}
}
