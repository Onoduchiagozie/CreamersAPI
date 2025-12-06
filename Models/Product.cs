using System.ComponentModel.DataAnnotations;

namespace AdonisAPI.Models
{

    public class Product
    {
        [Required]
        public string ProductImageBase64 { get; set; } // Base64 string (required)

        [Key]
        public string Id { get; set; } // Primary Key (auto-generated)

        [Required]
        public string Name { get; set; } // Required

         public int? Cost { get; set; } // Optional
        public string? Location { get; set; } // Optional
         public string? Description { get; set; } // Optional
        public string? Seller { get; set; } // Optional
    }
    // {
    //
    //          public string ProductImageBase64 { get; set; } // base64 string
    //
    //
    //     
    //     
    //     
    //     
    //     [Key]
    //     public string Id { get; set; } // Primary Key (Auto-generated)
    //
    //     public string Name { get; set; }
    //
    //     public string Price { get; set; }
    //     public int Cost { get; set; }
    //
    //     public string Location { get; set; }
    //     
    //
    //     // public List<string> Reviews { get; set; }
    //
    //     public string ProductImage { get; set; }
    //     // Store arrays as JSON strings in SQL Server
    //     public string Description { get; set; } // Stored as JSON (string format)
    //     public string Seller { get; set; } // Stored as JSON (string format)
    // }
}
