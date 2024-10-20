using PortfolioMicroService.Models;
using PortfolioMicroService.Repositories;

namespace PortfolioMicroService.Services
{
    public class UserPortfolioService
    {
        private readonly IUserPortfolioRepository _userPortfolioRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly CommonServices _commonServices;

        public UserPortfolioService(IUserPortfolioRepository userPortfolioRepository, IPositionRepository positionRepository, CommonServices commonServices)
        {
            _userPortfolioRepository = userPortfolioRepository;
            _positionRepository = positionRepository;
            _commonServices = commonServices;
        }

        public async Task<IEnumerable<UserPortfolio>> GetAllUserPortfoliosAsync()
        {
            return await _userPortfolioRepository.GetAllAsync();
        }

        public async Task<UserPortfolio> GetUserPortfolioByUserIdAsync(Guid userId)
        {
            return await _userPortfolioRepository.GetByUserIdAsync(userId);
        }

        public async Task AddUserPortfolioAsync(UserPortfolio userPortfolio)
        {
            await _userPortfolioRepository.AddAsync(userPortfolio);
        }

        public async Task UpdateUserPortfolioAsync(UserPortfolio userPortfolio){
            await _userPortfolioRepository.UpdateAsync(userPortfolio);
        }

        public async Task UpdateUserPortfolioByUserIdAsync(Guid userId)
        {
            var userPortfolio = await _userPortfolioRepository.GetByUserIdAsync(userId);

            var positions = await _positionRepository.GetPositionsByPortfolioIdAsync(userPortfolio.PortfolioID);
            
            foreach (var position in positions)
            {
                decimal updatedPrice = 0m;

                var details = await _commonServices.GetCurrencyPairDetails(position.CurrencyPairID);

                if (details.CategoryName.Equals("Forex", StringComparison.OrdinalIgnoreCase))
                {
                    updatedPrice = await _commonServices.FetchForexRateAsync(details.BaseCurrency, details.QuoteCurrency);
                }
                else if (details.CategoryName.Equals("Crypto", StringComparison.OrdinalIgnoreCase))
                {
                    updatedPrice = await _commonServices.FetchCryptoRateAsync(details.BaseCurrency, details.QuoteCurrency);
                }
                else if (details.CategoryName.Equals("Stocks", StringComparison.OrdinalIgnoreCase))
                {
                    updatedPrice = await _commonServices.FetchStockRateAsync(details.BaseCurrency);
                }

                position.CurrentValueUSD = updatedPrice * position.Amount;
                
                await _positionRepository.UpdateAsync(position);
            }

            userPortfolio.InvestedBalance = positions.Sum(p => p.CurrentValueUSD);

            await _userPortfolioRepository.UpdateAsync(userPortfolio);
        }

        public async Task DeleteUserPortfolioAsync(Guid userId)
        {
            await _userPortfolioRepository.DeleteByUserIdAsync(userId);
        }

        public async Task<UserPortfolio> UserPortfolioDeposit(Guid userId, decimal depositAmount){
            var userPortfolio = await GetUserPortfolioByUserIdAsync(userId);

            userPortfolio.AvailableWalletBalance += depositAmount;
            userPortfolio.UpdatedAt = DateTime.UtcNow;

            await _userPortfolioRepository.UpdateAsync(userPortfolio);

            return userPortfolio;
        }

        public async Task<UserPortfolio> UserPortfolioWithdraw(Guid userId, decimal withdrawAmount){
            var userPortfolio = await GetUserPortfolioByUserIdAsync(userId);

            userPortfolio.AvailableWalletBalance -= withdrawAmount;
            userPortfolio.UpdatedAt = DateTime.UtcNow;

            await _userPortfolioRepository.UpdateAsync(userPortfolio);

            return userPortfolio;
        }
    }
}
