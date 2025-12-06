using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AdonisAPI.Models;

public class CreamUser:IdentityUser
{
    [Required]
    public string FullName { get; set; }

    public bool IsSubscribed { get; set; }

    public bool IsSeller { get; set; }

    public ICollection<Favourite> Favorites { get; set; } = new List<Favourite>();
    // Navigation property for the user's favorites
 }