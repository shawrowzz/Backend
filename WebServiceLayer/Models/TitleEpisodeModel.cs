namespace WebServiceLayer.Models;

public class TitleEpisodeModel
{
    public string TConst { get; set; }
    public string ParentTConst { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
}