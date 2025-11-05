namespace WebServiceLayer.Models;

public class UserModel
{
    public string Url { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}