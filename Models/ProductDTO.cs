namespace AdonisAPI.Models;

public class ProductDTO
{
        public string Name { get; set; }
        public int Cost { get; set; }
         public string Description { get; set; }
         public string Location { get; set; }

        public string ProductImageBase64 { get; set; }
    
}