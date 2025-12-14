namespace DataServiceLayer.Models;

public class SearchHistory
{
    public int SearchId { get; set; }
    public int UserId { get; set; }
    public string? SearchQuery { get; set; }
    public string? SearchType { get; set; }
    public int? SearchResultsCount { get; set; }
    public DateTime SearchedAt { get; set; }
}
