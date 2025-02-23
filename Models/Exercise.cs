using System.ComponentModel.DataAnnotations;

namespace AdonisAPI.Models
{
 
    public class Exercise
    {
        [Key]
        public int Id { get; set; } // Primary Key (Auto-generated)

        [Required]
        public string ExerciseId { get; set; } // Stores the original "id" (e.g., "0022")

        public string Name { get; set; }

        public string BodyPart { get; set; }

        public string Equipment { get; set; }

        public string Target { get; set; }

        public string GifUrl { get; set; }

        // Store arrays as JSON strings in SQL Server
        public string Instructions { get; set; } // Stored as JSON (string format)
        public string SecondaryMuscles { get; set; } // Stored as JSON (string format)
    }
}
