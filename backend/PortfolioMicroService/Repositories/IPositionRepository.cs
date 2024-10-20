using PortfolioMicroService.Models;

namespace PortfolioMicroService.Repositories
{
    public interface IPositionRepository
    {
        Task<IEnumerable<Position>> GetAllAsync();
        Task<Position> GetByIdAsync(Guid id);
        Task AddAsync(Position position);
        Task UpdateAsync(Position position);
        Task DeleteAsync(Guid id);
        Task<Position> GetByCurrencyPairAndUserIdAsync(Guid CurrencyPairID, Guid UserID);
        Task<IEnumerable<Position>> GetPositionsByPortfolioIdAsync(Guid portfolioId);
        Task<IEnumerable<Position>> GetPositionsByUserIdAsync(Guid userId);
        Task<Position> GetPositionByCurrencyPairIdAsync(Guid currencyPairId, Guid portfolioId);
    }
}
