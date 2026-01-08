namespace AdonisAPI.Models.Order.DTOs;

public class PlaceOrderRequest
{
   
        public List<OrderItemRequest> Items { get; set; }
      
}