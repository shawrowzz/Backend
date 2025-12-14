namespace WebServiceLayer.Models;

public class TitleModel
{
    public string Url { get; set; } = string.Empty;
    public string TConst { get; set; } = string.Empty;
    public string TitleType { get; set; } = string.Empty;
    public string PrimaryTitle { get; set; } = string.Empty;
    public string OriginalTitle { get; set; } = string.Empty;
    public bool IsAdult { get; set; }
    public string StartYear { get; set; } = string.Empty;
    public string EndYear { get; set; } = string.Empty;
    public int? RuntimeMinutes { get; set; }
    public string Plot { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Runtime { get; set; } = string.Empty;
    public string Rated { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Released { get; set; } = string.Empty;
}