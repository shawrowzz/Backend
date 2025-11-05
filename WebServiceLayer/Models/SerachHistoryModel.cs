namespace WebServiceLayer.Models;

public class SearchHistoryModel
{
    public string Url { get; set; }
    public int SearchId { get; set; }
    public int UserId { get; set; }
    public string SearchQuery { get; set; }
    public string SearchType { get; set; }
    public DateTime SearchedAt { get; set; }
}