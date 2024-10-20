namespace PortfolioMicroService.Models{
    public class WithdrawRequest
    {
        public Guid UserId { get; set; }
        public decimal WithdrawAmount { get; set; }
    }
}