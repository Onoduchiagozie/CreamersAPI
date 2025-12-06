namespace AdonisAPI.Models;

public class UserResponse
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public CreamUser Customer { get; set; }
    public IEnumerable<string> Errors { get; set; }
}