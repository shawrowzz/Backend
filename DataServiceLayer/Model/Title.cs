namespace DataServiceLayer.Models;

public class Title
{
    public string TConst { get; set; } = string.Empty;
    public string TitleType { get; set; } = string.Empty;
    public string PrimaryTitle { get; set; } = string.Empty;
    public string OriginalTitle { get; set; } = string.Empty;
    public bool IsAdult { get; set; }
    public string StartYear { get; set; } = string.Empty;
    public string EndYear { get; set; } = string.Empty;
    public int? RuntimeMinutes { get; set; }
}