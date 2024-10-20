using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioMicroService.Models
{
    public class Position
    {
        [Key]
        public Guid PositionID { get; set; }

        [Required]
        [ForeignKey("UserPortfolio")]
        public Guid PortfolioID { get; set; }  // Foreign key to UserPortfolio

        [Required]
        public Guid CurrencyPairID { get; set; }  // Foreign key or identifier for the currency pair or crypto asset

        [Required]
        public decimal Amount { get; set; }  // Amount of the asset held

        [Required]
        public decimal CurrentValueUSD { get; set; }  // Current USD value of the position

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
