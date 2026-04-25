namespace HackerNewsService.Models;

public class StoryModel
{
	public string by { get; set; } = string.Empty;
	public int descendants { get; set; }
	public int id { get; set; }
	public int[] kids { get; set; } = [];
	public int[] parts { get; set; } = [];
	public int poll { get; set; }
	public int score { get; set; }
	public string text { get; set; } = string.Empty;
	public long time { get; set; }
	public string title { get; set; } = string.Empty;
	public string type { get; set; } = string.Empty;
}
