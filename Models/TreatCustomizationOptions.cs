namespace AdonisAPI.Models;

public class TreatCustomizationOptions
{
        public int Id { get; set; }
        public int CustomizationGroupId { get; set; }
        public TreatCustomizationGroup TreatCustomizationGroup { get; set; } = null!;
        public string Name { get; set; } = null!;         // "Normal", "Extra Sweet", "Vanilla"
        public int PriceIncrement { get; set; }  // money added when chosen
        public int SortOrder { get; set; }
}