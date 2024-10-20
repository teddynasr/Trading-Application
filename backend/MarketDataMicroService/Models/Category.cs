using System.ComponentModel.DataAnnotations;

namespace MarketDataMicroService.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }  // Forex, Crypto, Stocks

        [MaxLength(200)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
