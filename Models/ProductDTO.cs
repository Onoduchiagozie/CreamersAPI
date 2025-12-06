namespace AdonisAPI.Models;

public class ProductDTO
{
    
        public string Name { get; set; }
        public int Price { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public IFormFile Image { get; set; }
    

}