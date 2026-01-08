namespace AdonisAPI.Models.Order;

public class OrderItemCustomization
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; }

    public string GroupName { get; set; }
    public string OptionName { get; set; }
    public int PriceIncrement { get; set; }
}