using MarketDataMicroService.Models;

namespace MarketDataMicroService.Repositories
{
    public interface ICurrencyPairRepository
    {
        Task<IEnumerable<CurrencyPair>> GetAllAsync();
        Task<IEnumerable<CurrencyPair>> GetByCategoryNameAsync(string categoryName);
        Task<CurrencyPair> GetByIdAsync(Guid id);
        Task<(string BaseCurrency, string QuoteCurrency, string CategoryName)> GetCurrencyPairDetailsAsync(Guid currencyPairId);
        Task AddAsync(CurrencyPair currencyPair);
        Task UpdateAsync(CurrencyPair currencyPair);
        Task DeleteAsync(Guid id);
    }
}
