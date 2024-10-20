using Microsoft.EntityFrameworkCore;
using PortfolioMicroService.Data;
using PortfolioMicroService.Models;

namespace PortfolioMicroService.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly PortfolioDBContext _context;

        public PositionRepository(PortfolioDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Position>> GetAllAsync()
        {
            return await _context.Positions.ToListAsync();
        }

        public async Task<Position> GetByIdAsync(Guid id)
        {
            return await _context.Positions.FindAsync(id);
        }

        public async Task AddAsync(Position position)
        {
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Position position)
        {
            _context.Positions.Update(position);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position != null)
            {
                _context.Positions.Remove(position);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Position> GetByCurrencyPairAndUserIdAsync(Guid currencyPairID, Guid userID)
        {
            var UserPortfolio = await _context.UserPortfolios.FirstOrDefaultAsync(up => up.UserID == userID);

            return await _context.Positions
                .FirstOrDefaultAsync(p => p.CurrencyPairID == currencyPairID && p.PortfolioID == UserPortfolio.PortfolioID);
        }

        public async Task<IEnumerable<Position>> GetPositionsByPortfolioIdAsync(Guid portfolioId)
        {
            return await _context.Positions
                .Where(p => p.PortfolioID == portfolioId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Position>> GetPositionsByUserIdAsync(Guid userId)
        {
            return await (from position in _context.Positions
                        join portfolio in _context.UserPortfolios
                        on position.PortfolioID equals portfolio.PortfolioID
                        where portfolio.UserID == userId
                        select position)
                        .ToListAsync();
        }

        public async Task<Position> GetPositionByCurrencyPairIdAsync(Guid currencyPairID, Guid portfolioId)
        {
            return await _context.Positions
                .FirstOrDefaultAsync(p => p.CurrencyPairID == currencyPairID && p.PortfolioID == portfolioId);
        }
    }
}
