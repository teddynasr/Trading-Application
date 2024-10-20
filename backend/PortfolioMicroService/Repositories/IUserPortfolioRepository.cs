using PortfolioMicroService.Models;

namespace PortfolioMicroService.Repositories
{
    public interface IUserPortfolioRepository
    {
        Task<IEnumerable<UserPortfolio>> GetAllAsync();
        Task<UserPortfolio> GetByIdAsync(Guid id);
        Task<UserPortfolio> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserPortfolio userPortfolio);
        Task UpdateAsync(UserPortfolio userPortfolio);
        Task DeleteByUserIdAsync(Guid userId);
    }
}
