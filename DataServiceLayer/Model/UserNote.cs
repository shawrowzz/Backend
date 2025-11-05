namespace DataServiceLayer.Models;

public class UserNote
{
    public int NoteId { get; set; }
    public int UserId { get; set; }
    public string? TitleTConst { get; set; }
    public string? NoteText { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}