using Microsoft.EntityFrameworkCore;
using MarketDataMicroService.Models;
using MarketDataMicroService.Data;

namespace MarketDataMicroService.Repositories
{
    public class CurrencyPairRepository : ICurrencyPairRepository
    {
        private readonly MarketDataDBContext _context;

        public CurrencyPairRepository(MarketDataDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurrencyPair>> GetAllAsync()
        {
            return await _context.CurrencyPairs.ToListAsync();
        }

        public async Task<IEnumerable<CurrencyPair>> GetByCategoryNameAsync(string categoryName)
        {
            return await (from cp in _context.CurrencyPairs
                  join c in _context.Categories on cp.CategoryId equals c.CategoryId
                  where c.CategoryName == categoryName && cp.IsActive
                  select cp)
                 .ToListAsync();
        }

        public async Task<CurrencyPair> GetByIdAsync(Guid id)
        {
            return await _context.CurrencyPairs.FindAsync(id);
        }

        public async Task<(string BaseCurrency, string QuoteCurrency, string CategoryName)> GetCurrencyPairDetailsAsync(Guid currencyPairId)
        {
            var result = await (from cp in _context.CurrencyPairs
                                join c in _context.Categories on cp.CategoryId equals c.CategoryId
                                where cp.PairId == currencyPairId
                                select new
                                {
                                    cp.BaseCurrency,
                                    cp.QuoteCurrency,
                                    c.CategoryName
                                })
                                .FirstOrDefaultAsync();

            if (result == null)
                return (null, null, null);

            return (result.BaseCurrency, result.QuoteCurrency, result.CategoryName);
        }

        public async Task AddAsync(CurrencyPair currencyPair)
        {
            await _context.CurrencyPairs.AddAsync(currencyPair);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CurrencyPair currencyPair)
        {
            _context.CurrencyPairs.Update(currencyPair);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var currencyPair = await GetByIdAsync(id);
            if (currencyPair != null)
            {
                _context.CurrencyPairs.Remove(currencyPair);
                await _context.SaveChangesAsync();
            }
        }
    }
}
