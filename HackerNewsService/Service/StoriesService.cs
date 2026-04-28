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
	private static SemaphoreSlim _semaphore = new SemaphoreSlim(5);
	public async Task<List<int>> GetBestStoriesIds()
	{
		var bestStories = new List<int>();

		// Limit simultaneous requests
		await _semaphore.WaitAsync();

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

		_semaphore.Release();

		return bestStories!;
	}

	public async Task<List<StoryModel>> GetBestStories(int num)
	{
		var bestStories = new List<StoryModel>();

		//Current stories from the API
		var bestStoriesIds = await GetBestStoriesIds();

		//Ids saved in cache
		var cachedStoriesIds = cache.GetBestStoriesIds();

		//Ids that was included
		var includedIds = bestStoriesIds.Except(cachedStoriesIds).ToList();
		//Ids that was removed
		var removedIds = cachedStoriesIds.Except(bestStoriesIds).ToList();

		bool hasChanges = false;

		if ((includedIds != null && includedIds.Count > 0)
			||(removedIds != null && removedIds.Count > 0))
		{
			hasChanges = true;
		}

		if (!hasChanges)
		{
			bestStories = cache.GetBestStories(num);
			if (bestStories.Count > 0)
			{
				return bestStories;
			}
		}

		//Avoiding overload on calling hackernews endpoints
		if (cache.IsUpdating())
		{
			return [];
		}

		// Limit simultaneous requests
		await _semaphore.WaitAsync();

		cache.SetUpdating(true);

		//Remove old stories
		foreach (var removed in removedIds!)
		{
			cache.RemoveStory(removed);
			cachedStoriesIds.Remove(removed);
		}

		foreach (var id in includedIds!)
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

		cachedStoriesIds.AddRange(includedIds);
		cache.SetBestStoriesIds(cachedStoriesIds);

		cache.SetUpdating(false);

		cache.CacheUpdated();

		bestStories = cache.GetBestStories(num);

		_semaphore.Release();

		return bestStories;
	}

	public void SetCacheLifetime(int seconds)
	{
		cache.SetCacheLifetime(seconds);
	}
}
