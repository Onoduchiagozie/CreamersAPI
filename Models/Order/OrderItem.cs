namespace AdonisAPI.Models.Order;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string ProductId { get; set; }
    public string ProductName { get; set; }

    public int BasePrice { get; set; }
    public int Quantity { get; set; }
    public int LineTotal { get; set; }

    public ICollection<OrderItemCustomization> Customizations { get; set; }
}

