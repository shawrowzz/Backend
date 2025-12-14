namespace WebServiceLayer.Models;

public class CreateBookmarkModel
{
    public string TitleTConst { get; set; } = string.Empty;     
    public string PersonNConst { get; set; } = string.Empty;    
    public string BookmarkType { get; set; } = string.Empty;
    public string Folder { get; set; } = "General";
    public string Notes { get; set; } = string.Empty;
}