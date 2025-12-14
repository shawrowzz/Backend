using System.ComponentModel.DataAnnotations.Schema;

namespace DataServiceLayer.Models;

[Table("omdb_datas", Schema = "public")]
public class OmdbData
{
    public string TConst { get; set; } = string.Empty;
    public string? Episode { get; set; }
    public string? Awards { get; set; }
    public string? Plot { get; set; }
    public string? SeriesId { get; set; }
    public string? Rated { get; set; }
    public string? ImdbRating { get; set; }
    public string? Runtime { get; set; }
    public string? Language { get; set; }
    public string? Released { get; set; }
    public string? Response { get; set; }
    public string? Writer { get; set; }
    public string? Genre { get; set; }
    public string? Title { get; set; }
    public string? Country { get; set; }
    public string? Dvd { get; set; }
    public string? Production { get; set; }
    public string? Season { get; set; }
    public string? Type { get; set; }
    public string? Poster { get; set; }
}