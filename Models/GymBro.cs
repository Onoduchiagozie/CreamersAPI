using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AdonisAPI.Models;

public class GymBro:IdentityUser
{
    [Key]
    public string Id { get; set; }  // Primary Key

    [Required]
    public string FullName { get; set; }  
    public string Goal { get; set; }  
    

    // Navigation property for the user's favorites
    public List<Favourite> Favorites { get; set; } = new List<Favourite>();
}