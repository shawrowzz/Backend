namespace WebServiceLayer.Dto;

public class UserRatingDto
{
    public int RatingId { get; set; }
    public int UserId { get; set; }
    public string TitleTConst { get; set; }
    public int Rating { get; set; }
    public DateTime RatedAt { get; set; }
}