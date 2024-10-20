using System.Text.Json;
using MarketDataMicroService.Repositories;
using MarketDataMicroService.Responses;
using MarketDataMicroService.Models;
using StackExchange.Redis;

namespace MarketDataMicroService.Services
{
    public class CurrencyPairService
    {
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly IDatabase _redisCache;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly int _cacheExpiryTime=10;

        public CurrencyPairService(ICurrencyPairRepository currencyPairRepository, IDatabase redisCache, HttpClient httpClient, IConfiguration configuration)
        {
            _currencyPairRepository = currencyPairRepository;
            _redisCache=redisCache;
            _httpClient=httpClient;
            _apiKey=configuration["ForexApi:ApiKey"];
        }

        public async Task<IEnumerable<CurrencyPair>> GetAllAsync()
        {
            return await _currencyPairRepository.GetAllAsync();
        }

        public async Task<IEnumerable<CurrencyPair>> GetByCategoryNameAsync(string categoryName)
        {
            return await _currencyPairRepository.GetByCategoryNameAsync(categoryName);
        }

        public async Task<CurrencyPair> GetByIdAsync(Guid id)
        {
            return await _currencyPairRepository.GetByIdAsync(id);
        }

        public async Task<(string BaseCurrency, string QuoteCurrency, string CategoryName)> GetCurrencyPairDetailsAsync(Guid currencyPairId)
        {
            return await _currencyPairRepository.GetCurrencyPairDetailsAsync(currencyPairId);
        }

        public async Task AddAsync(CurrencyPair currencyPair)
        {
            await _currencyPairRepository.AddAsync(currencyPair);
        }

        public async Task UpdateAsync(CurrencyPair currencyPair)
        {
            await _currencyPairRepository.UpdateAsync(currencyPair);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _currencyPairRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ForexRate>> GetAtMarketForexRate(string baseCurrency, string quoteCurrency){
            string endpoint=$"https://api.tiingo.com/tiingo/fx/{baseCurrency}{quoteCurrency}/top?token={_apiKey}";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode){
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var forexData = JsonSerializer.Deserialize<List<ForexRate>>(jsonResponse);
                return forexData;
            }

            return null;
        }

        public async Task<IEnumerable<HistoryForexRate>> GetHistoryForexRate(string baseCurrency, string quoteCurrency, string startDate, string endDate, string candleSize)
        {
            string cacheKey = $"{baseCurrency}_{quoteCurrency}_{startDate}_{endDate}_{candleSize}";

            var cachedData = await _redisCache.StringGetAsync(cacheKey);
            if (cachedData.HasValue)
            {
                return JsonSerializer.Deserialize<List<HistoryForexRate>>(cachedData);
            }

            string endpoint = $"https://api.tiingo.com/tiingo/fx/{baseCurrency}{quoteCurrency}/prices?startDate={startDate}&endDate={endDate}&resampleFreq={candleSize}&token={_apiKey}";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var forexData = JsonSerializer.Deserialize<List<HistoryForexRate>>(jsonResponse);
                await _redisCache.StringSetAsync(cacheKey, jsonResponse, TimeSpan.FromMinutes(_cacheExpiryTime));
                return forexData;
            }

            return null;
        }

        public async Task<IEnumerable<CryptoRate>> GetAtMarketCryptoRate(string baseCurrency, string quoteCurrency){
            string endpoint=$"https://api.tiingo.com/tiingo/crypto/top?tickers={baseCurrency}{quoteCurrency}&token={_apiKey}";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode){
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var cryptoData = JsonSerializer.Deserialize<List<CryptoRate>>(jsonResponse);
                return cryptoData;
            }

            return null;
        }

        public async Task<IEnumerable<HistoryCryptoRate>> GetHistoryCryptoRate(string baseCurrency, string quoteCurrency, string startDate, string endDate, string candleSize)
        {
            string cacheKey = $"{baseCurrency}_{quoteCurrency}_{startDate}_{endDate}_{candleSize}";

            var cachedData = await _redisCache.StringGetAsync(cacheKey);
            if (cachedData.HasValue)
            {
                return JsonSerializer.Deserialize<List<HistoryCryptoRate>>(cachedData);
            }

            string endpoint = $"https://api.tiingo.com/tiingo/crypto/prices?tickers={baseCurrency}{quoteCurrency}&startDate={startDate}&endDate={endDate}&resampleFreq={candleSize}&token={_apiKey}";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var cryptoData = JsonSerializer.Deserialize<List<HistoryCryptoRate>>(jsonResponse);
                await _redisCache.StringSetAsync(cacheKey, jsonResponse, TimeSpan.FromMinutes(_cacheExpiryTime));
                return cryptoData;
            }

            return null;
        }

        public async Task<IEnumerable<StockRate>> GetAtMarketStockRate(string stock){
            string endpoint=$"https://api.tiingo.com/iex?tickers={stock}&token={_apiKey}";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode){
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var stockData = JsonSerializer.Deserialize<List<StockRate>>(jsonResponse);
                return stockData;
            }

            return null;
        }

        public async Task<IEnumerable<HistoryStockRate>> GetHistoryStockRate(string stock, string startDate, string endDate, string candleSize)
        {
            string cacheKey = $"{stock}_{startDate}_{endDate}_{candleSize}";

            var cachedData = await _redisCache.StringGetAsync(cacheKey);
            if (cachedData.HasValue)
            {
                return JsonSerializer.Deserialize<List<HistoryStockRate>>(cachedData);
            }

            string endpoint = $"https://api.tiingo.com/iex/{stock}/prices?startDate={startDate}&endDate={endDate}&resampleFreq={candleSize}&token={_apiKey}";
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var stockData = JsonSerializer.Deserialize<List<HistoryStockRate>>(jsonResponse);
                await _redisCache.StringSetAsync(cacheKey, jsonResponse, TimeSpan.FromMinutes(_cacheExpiryTime));
                return stockData;
            }

            return null;
        }
    }
}
