using PortfolioMicroService.Models;
using PortfolioMicroService.Repositories;

namespace PortfolioMicroService.Services
{
    public class PositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IUserPortfolioRepository _userPortfolioRepository;
        private readonly CommonServices _commonServices;

        public PositionService(IPositionRepository positionRepository, IUserPortfolioRepository userPortfolioRepository, CommonServices commonServices)
        {
            _positionRepository = positionRepository;
            _userPortfolioRepository = userPortfolioRepository;
            _commonServices = commonServices;
        }

        public async Task<IEnumerable<Position>> GetAllPositionsAsync()
        {
            return await _positionRepository.GetAllAsync();
        }
        public async Task<Position> GetPositionByIdAsync(Guid id)
        {
            return await _positionRepository.GetByIdAsync(id);
        }

        public async Task AddPositionAsync(Position position)
        {
            await _positionRepository.AddAsync(position);
        }

        public async Task UpdatePositionAsync(Position position)
        {
            await _positionRepository.UpdateAsync(position);
        }

        public async Task DeletePositionAsync(Guid id)
        {
            await _positionRepository.DeleteAsync(id);
        }

        public async Task<Position> GetByCurrencyPairAndUserIdAsync(Guid CurrencyPairID, Guid UserID){
            return await _positionRepository.GetByCurrencyPairAndUserIdAsync(CurrencyPairID, UserID);
        }

        public async Task<IEnumerable<Position>> GetPositionsByUserIdAsync(Guid userId){
            return await _positionRepository.GetPositionsByUserIdAsync(userId);
        }

        public async Task<Position> GetPositionByCurrencyPairIdAsync(Guid currencyPair, Guid userId){
            var userPortfolio = await _userPortfolioRepository.GetByUserIdAsync(userId);
            return await _positionRepository.GetPositionByCurrencyPairIdAsync(currencyPair, userPortfolio.PortfolioID);
        }
        
        public async Task<Position> GetUpdatedPositionBycurrencyPairIdAsync(Guid currencyPair, Guid userId){
            var userPortfolio = await _userPortfolioRepository.GetByUserIdAsync(userId);
            var position = await _positionRepository.GetPositionByCurrencyPairIdAsync(currencyPair, userPortfolio.PortfolioID);
            
            if(position==null)
            {
                return null;
            }

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

            var positions = await _positionRepository.GetPositionsByPortfolioIdAsync(position.PortfolioID);
            decimal totalInvestedValue = 0m;
            foreach (var pos in positions)
            {
                totalInvestedValue += pos.CurrentValueUSD;
            }

            userPortfolio.InvestedBalance = totalInvestedValue;
            userPortfolio.UpdatedAt = DateTime.UtcNow;
            await _userPortfolioRepository.UpdateAsync(userPortfolio);

            return position;
        }
    }
}
