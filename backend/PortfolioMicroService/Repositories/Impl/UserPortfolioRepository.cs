using Microsoft.EntityFrameworkCore;
using PortfolioMicroService.Data;
using PortfolioMicroService.Models;

namespace PortfolioMicroService.Repositories
{
    public class UserPortfolioRepository : IUserPortfolioRepository
    {
        private readonly PortfolioDBContext _context;

        public UserPortfolioRepository(PortfolioDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserPortfolio>> GetAllAsync()
        {
            return await _context.UserPortfolios.ToListAsync();
        }

        public async Task<UserPortfolio> GetByIdAsync(Guid id)
        {
            return await _context.UserPortfolios.FindAsync(id);
        }

        public async Task<UserPortfolio> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserPortfolios.FirstOrDefaultAsync(up => up.UserID == userId);
        }

        public async Task AddAsync(UserPortfolio userPortfolio)
        {
            await _context.UserPortfolios.AddAsync(userPortfolio);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserPortfolio userPortfolio)
        {
            _context.UserPortfolios.Update(userPortfolio);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var userPortfolio = await _context.UserPortfolios.FirstOrDefaultAsync(up => up.UserID == userId);

            if (userPortfolio != null)
            {
                _context.UserPortfolios.Remove(userPortfolio);
                await _context.SaveChangesAsync();
            }
        }
    }
}
