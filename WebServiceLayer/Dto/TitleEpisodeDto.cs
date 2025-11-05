namespace WebServiceLayer.Dto;

public class TitleEpisodeDto
{
    public string TConst { get; set; }
    public string ParentTConst { get; set; }
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
}