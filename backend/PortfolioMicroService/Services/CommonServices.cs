using MarketDataMicroService.Responses;
using PortfolioMicroService.Models;
using PortfolioMicroService.Repositories;

namespace PortfolioMicroService.Services{
    public class CommonServices{
        private readonly IPositionRepository _positionRepository;
        private readonly HttpClient _httpClient;

        public CommonServices(IPositionRepository positionRepository, HttpClient httpClient)
        {
            _positionRepository = positionRepository;
            _httpClient = httpClient;
        }

        public async Task<(string BaseCurrency, string QuoteCurrency, string CategoryName)> GetCurrencyPairDetails(Guid currencyPairId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5065/api/currencyPair/details?currencyPairId={currencyPairId}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CurrencyPairDetails>();
                if (result != null)
                {
                    return (result.BaseCurrency, result.QuoteCurrency, result.CategoryName);
                }
            }

            return (null, null, null);
        }

        public async Task<decimal> FetchForexRateAsync(string baseCurrency, string quoteCurrency)
        {
            var url = $"http://localhost:5065/api/currencyPair/forex-rate?baseCurrency={baseCurrency}&quoteCurrency={quoteCurrency}";

            var response = await _httpClient.GetAsync(url);

            var forexRate = await response.Content.ReadFromJsonAsync<List<ForexRate>>();
            return forexRate[0].MidPrice;
        }

        public async Task<decimal> FetchCryptoRateAsync(string baseCurrency, string quoteCurrency)
        {
            var url = $"http://localhost:5065/api/currencyPair/crypto-rate?baseCurrency={baseCurrency}&quoteCurrency={quoteCurrency}";

            var response = await _httpClient.GetAsync(url);

            var cryptoRate = await response.Content.ReadFromJsonAsync<List<CryptoRate>>();
            return cryptoRate[0].TopOfBookData[0].LastPrice;
        }

        public async Task<decimal> FetchStockRateAsync(string stock)
        {
            var url = $"http://localhost:5065/api/currencyPair/stock-rate?stock={stock}";

            var response = await _httpClient.GetAsync(url);

            var stockRate = await response.Content.ReadFromJsonAsync<List<StockRate>>();
            return stockRate[0].Last;
        }
    }
}