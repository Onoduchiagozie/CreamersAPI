namespace AdonisAPI.Models;

public class ProductDTO
{
        public string Name { get; set; }
        public int Cost { get; set; }
         public string Description { get; set; }
         public string Location { get; set; }

        public string ProductImageBase64 { get; set; }
        
        public List<CreateCustomizationGroupDto> Customizations { get; set; }= new();
}

public class CreateCustomizationGroupDto
{
    public string Name { get; set; }            // Sweetness, Flavour, Toppings
    public bool IsRequired { get; set; }
    public int MaxSelections { get; set; }
    public List<CreateCustomizationOptionDto> Options { get; set; }
}

public class CreateCustomizationOptionDto
{
    public string Name { get; set; }             // Vanilla, Banana, Cashew
    public int PriceIncrement { get; set; }
}


