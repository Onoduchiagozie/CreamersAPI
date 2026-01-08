namespace AdonisAPI.Models.Order;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int Subtotal { get; set; }
    public int Tax { get; set; }
    public int Total { get; set; }

    public string Status { get; set; } = "Pending";

  //  public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    
    public List<OrderItem> Items { get; set; } = new();


}

public class OrderSummaryDto
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Total { get; set; }
    public string Status { get; set; }
    public int TotalItems { get; set; }
}
