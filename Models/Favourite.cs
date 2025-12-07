using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdonisAPI.Models;
public class Favourite
{
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
         [ForeignKey("CreamUser")]
        public string CreamUserId { get; set; }
        public CreamUser CreamUser { get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}