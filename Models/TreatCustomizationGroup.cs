namespace AdonisAPI.Models;

public class TreatCustomizationGroup
{
 
        public int Id { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string Name { get; set; } = null!;      // "Sweetness", "Flavour", "Toppings"
        public bool IsRequired { get; set; } = false;  // required
        public int MaxSelections { get; set; } = 1;    // 1 for radio, >1 for multi-select
        public int SortOrder { get; set; }
        public ICollection<TreatCustomizationOptions> Options { get; set; } = new List<TreatCustomizationOptions>();
 

}