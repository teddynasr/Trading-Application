using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderMicroService.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserID { get; set; }

        [Required]
        public Guid CurrencyPairID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal OrderAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal OrderUnitPrice { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [RegularExpression("^(buy|sell)$", ErrorMessage = "OrderType must be either 'buy' or 'sell'.")]
        public string OrderType { get; set; }
    }
}
