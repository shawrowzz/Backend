namespace DataServiceLayer.Models;

public class UserBookmark
{
    public int BookmarkId { get; set; }
    public int UserId { get; set; }
    public string? TitleTConst { get; set; }
    public string? PersonNConst { get; set; } 
    public string? BookmarkType { get; set; }
    public string? Folder { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}