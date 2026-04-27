using HackerNewsService.Models;

namespace HackerNewsService.Infra;

public class Cache : ICache
{
	private Dictionary<int, StoryModel> _bestStories = [];
	private List<int> _bestStoriesIds = [];
	private bool _isUpdating = false;
	private object _lock = new object();
	private int _cacheLifetimeInSeconds = 300; //default is 5 minutes
	private DateTime? _lastCacheUpdate = null;

	public void CacheUpdated()
	{
		_lastCacheUpdate = DateTime.UtcNow;
	}

	public void SetCacheLifetime(int seconds)
	{
		if (seconds > 0)
			_cacheLifetimeInSeconds = seconds;
	}

	public List<StoryModel> GetBestStories(int num)
	{
		if (_lastCacheUpdate == null)
		{
			_lastCacheUpdate = DateTime.UtcNow;
		}
		else
		{
			var timeElapsed = DateTime.UtcNow - _lastCacheUpdate;
			//Check if the cache if expired
			if (timeElapsed != null
				&& timeElapsed.Value.TotalSeconds > _cacheLifetimeInSeconds)
			{
				_bestStories.Clear();
				_bestStoriesIds.Clear();
			}
		}

		if (_bestStories.Count > 0)
		{
			//If the list has less than 'num', get all
			if (num > _bestStories.Count) num = _bestStories.Count;

			var bestStories = _bestStories.Values.ToList();
			var orderedStories = bestStories.OrderByDescending(s => s.score).Take(num).ToList();
			return orderedStories;
		}
		return [];
	}

	public StoryModel? GetStoryById(int id)
	{
		if (_bestStories.ContainsKey(id))
		{
			return _bestStories[id];
		}
		return null;
	}

	public void AddStory(StoryModel story)
	{
		_bestStories.Add(story.id, story);
	}

	//public void UpdateStory(int id, StoryModel story)
	//{
	//	//Avoiding race conditions (concurrency)
	//	while (_isUpdating)
	//	{
	//		Task.Delay(100);
	//	}

	//	lock (_lock)
	//	{
	//		_isUpdating = true;
	//		if (_bestStories.ContainsKey(id))
	//		{
	//			_bestStories[id] = story;
	//		}
	//		_isUpdating = false;
	//	}

	//}

	public void RemoveStory(int id)
	{
		if (_bestStories.ContainsKey(id))
		{
			_bestStories.Remove(id);
		}
	}

	public List<int> GetBestStoriesIds()
	{
		return _bestStoriesIds;
	}

	public void SetBestStoriesIds(List<int> bestStoriesIds)
	{
		_bestStoriesIds = bestStoriesIds;
	}

	public bool IsUpdating()
	{
		return _isUpdating;
	}

	public void SetUpdating(bool updFlag)
	{
		//thread safety
		lock (_lock)
		{
			_isUpdating = updFlag;
		}
	}
}
