using System.ComponentModel.DataAnnotations;

namespace PortfolioMicroService.Models
{
    public class UserPortfolio
    {
        [Key]
        public Guid PortfolioID { get; set; }

        [Required]
        public Guid UserID { get; set; }  // Reference to UserService's UserID

        [Required]
        public decimal AvailableWalletBalance { get; set; }  // Funds available for new investments

        [Required]
        public decimal InvestedBalance { get; set; }  // Funds already invested

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
