using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdonisAPI.Models;
public class Favourite
{
    [Key]
    public int Id { get; set; }  // Primary Key

    // Foreign Key for User (GymBro)
    [ForeignKey("CreamUser")]
    public string CreamUserId { get; set; }  // Ensure it matches GymBro's primary key type
    public  CreamUser CreamUser { get; set; }  // Navigation property

    // Foreign Key for Exercise
    [ForeignKey("Product")]
    public string ProductId { get; set; }
    public Product Product { get; set; }  // Navigation property
    public string ImagePath { get; set; }
}