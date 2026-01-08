namespace AdonisAPI.Models.Order;

public class EmailOrder
{
    public class OrderEmailModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public List<OrderEmailItemModel> Items { get; set; } = new();
    }

    public class OrderEmailItemModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}

