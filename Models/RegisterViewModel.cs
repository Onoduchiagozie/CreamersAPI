using System.ComponentModel.DataAnnotations;

namespace AdonisAPI.Models;

public class RegisterViewModel
{
    
    public string Username { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }
     
}