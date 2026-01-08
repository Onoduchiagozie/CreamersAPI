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
         public string Category { get; set; } // Optional
        public string? Location { get; set; } // Optional
         public string? Description { get; set; } // Optional
        public string? SellerId { get; set; } // Optional
         
        // Navigation property
        public CreamUser Seller { get; set; }
        public ICollection<Favourite> FavouritedBy { get; set; }
        public ICollection<TreatCustomizationGroup> CustomizationGroups { get; set; } = new List<TreatCustomizationGroup>();


    }
   

}
