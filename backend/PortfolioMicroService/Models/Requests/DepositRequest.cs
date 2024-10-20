namespace PortfolioMicroService.Models{
    public class DepositRequest
    {
        public Guid UserId { get; set; }
        public decimal DepositAmount { get; set; }
    }
}