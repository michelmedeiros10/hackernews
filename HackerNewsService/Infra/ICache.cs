using HackerNewsService.Models;

namespace HackerNewsService.Infra;

public interface ICache
{
	void CacheUpdated();
	void SetCacheLifetime(int seconds);
	List<StoryModel> GetBestStories(int num);
	StoryModel? GetStoryById(int id);
	void AddStory(StoryModel story);

	//void UpdateStory(int id, StoryModel story);
	void RemoveStory(int id);
	List<int> GetBestStoriesIds();
	void SetBestStoriesIds(List<int> bestStoriesIds);
	bool IsUpdating();
	void SetUpdating(bool updFlag);
}
