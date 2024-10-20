using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketDataMicroService.Models
{
    public class CurrencyPair
    {
        [Key]
        public Guid PairId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(10)]
        public string BaseCurrency { get; set; }

        [Required]
        [MaxLength(10)]
        public string QuoteCurrency { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
    }
}
