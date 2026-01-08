namespace AdonisAPI.Models;

 
    public class InitializePaymentRequest
    {
        public string Email { get; set; }
        public int Amount { get; set; } // in kobo
    }

