namespace AdonisAPI.Models;

public class Transaction
{
    public string Id { get; set; }= Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; }=DateTime.Now;
    public int Amount { get; set; }
    public Guid TransactionReference { get; set; }
    public string Email { get; set; }
}