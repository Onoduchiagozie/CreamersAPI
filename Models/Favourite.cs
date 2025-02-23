using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdonisAPI.Models;

public class Favourite
{
    [Key]
    public int Id { get; set; }  // Primary Key

    // Foreign Key for User (GymBro)
    [ForeignKey("GymBro")]
    public string UserId { get; set; }  // Ensure it matches GymBro's primary key type
    public GymBro GymBro { get; set; }  // Navigation property

    // Foreign Key for Exercise
    [ForeignKey("Exercise")]
    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; }  // Navigation property
    public string LocalImagePath { get; set; }
}