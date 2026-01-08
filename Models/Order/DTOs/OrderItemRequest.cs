namespace AdonisAPI.Models.Order.DTOs;

public class OrderItemRequest
{
      public string ProductId { get; set; }
        public int Quantity { get; set; }

       // public List<SelectedCustomizationDto> Customizations { get; set; }
 

}
// public class SelectedCustomizationDto
// {
//     public int CustomizationOptionId { get; set; }
// }
