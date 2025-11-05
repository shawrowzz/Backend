namespace WebServiceLayer.Dto;

public class UserNoteDto
{
    public int NoteId { get; set; }
    public int UserId { get; set; }
    public string TitleTConst { get; set; }
    public string NoteText { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}