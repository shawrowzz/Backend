namespace WebServiceLayer.Dto;

public class SearchHistoryDto
{
    public int SearchId { get; set; }
    public int UserId { get; set; }
    public string SearchQuery { get; set; }
    public string SearchType { get; set; }
    public DateTime SearchedAt { get; set; }
}