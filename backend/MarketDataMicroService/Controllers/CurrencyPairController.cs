using Microsoft.AspNetCore.Mvc;
using MarketDataMicroService.Responses;
using MarketDataMicroService.Models;
using MarketDataMicroService.Services;

namespace MarketDataMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyPairController : ControllerBase
    {
        private readonly CurrencyPairService _currencyPairService;

        public CurrencyPairController(CurrencyPairService currencyPairService)
        {
            _currencyPairService = currencyPairService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyPair>>> GetAll()
        {
            var currencyPairs = await _currencyPairService.GetAllAsync();
            return Ok(currencyPairs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyPair>> GetById(Guid id)
        {
            var currencyPair = await _currencyPairService.GetByIdAsync(id);
            if (currencyPair == null)
                return NotFound();

            return Ok(currencyPair);
        }

        [HttpGet("details")]
        public async Task<ActionResult> GetCurrencyPairDetails([FromQuery] Guid currencyPairId)
        {
            var details = await _currencyPairService.GetCurrencyPairDetailsAsync(currencyPairId);
            
            if (details.BaseCurrency == null)
            {
                return NotFound($"Currency pair details not found for ID {currencyPairId}");
            }

            return Ok(new
            {
                BaseCurrency = details.BaseCurrency,
                QuoteCurrency = details.QuoteCurrency,
                CategoryName = details.CategoryName
            });
        }

        [HttpGet("category/{categoryName}")]
        public async Task<ActionResult<IEnumerable<CurrencyPair>>> GetByCategoryName(string categoryName)
        {   
            categoryName=categoryName.ToLower();
            var currencyPairs = await _currencyPairService.GetByCategoryNameAsync(categoryName);
            
            if (currencyPairs == null || !currencyPairs.Any())
                return NotFound($"No currency pairs found for category '{categoryName}'.");

            return Ok(currencyPairs);
        }

        [HttpPost]
        public async Task<ActionResult> Add(CurrencyPair currencyPair)
        {
            await _currencyPairService.AddAsync(currencyPair);
            return CreatedAtAction(nameof(GetById), new { id = currencyPair.PairId }, currencyPair);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, CurrencyPair currencyPair)
        {
            if (id != currencyPair.PairId)
                return BadRequest();

            await _currencyPairService.UpdateAsync(currencyPair);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _currencyPairService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("forex-rate")]
        public async Task<ActionResult<IEnumerable<ForexRate>>> GetAtMarketForexRateAsync(
            [FromQuery] string baseCurrency, 
            [FromQuery] string quoteCurrency)
        {
            var forexRate = await _currencyPairService.GetAtMarketForexRate(baseCurrency, quoteCurrency);
            if (forexRate == null)
            {
                return NotFound("Forex rate not found.");
            }

            return Ok(forexRate);
        }

        [HttpGet("history-forex-rate")]
        public async Task<ActionResult<IEnumerable<HistoryForexRate>>> GetHistoryForexRateAsync(
            [FromQuery] string baseCurrency, 
            [FromQuery] string quoteCurrency, 
            [FromQuery] string startDate, 
            [FromQuery] string endDate, 
            [FromQuery] string candleSize) // min, hour, day
        {
            var historyForexRate = await _currencyPairService.GetHistoryForexRate(baseCurrency, quoteCurrency, startDate, endDate, candleSize);
            if (historyForexRate == null)
            {
                return NotFound("Forex rate not found.");
            }

            return Ok(historyForexRate);
        }

        [HttpGet("crypto-rate")]
        public async Task<ActionResult<IEnumerable<CryptoRate>>> GetAtMarketCryptoRateAsync(
            [FromQuery] string baseCurrency, 
            [FromQuery] string quoteCurrency)
        {
            var cryptoRate = await _currencyPairService.GetAtMarketCryptoRate(baseCurrency, quoteCurrency);
            if (cryptoRate == null)
            {
                return NotFound("Crypto rate not found.");
            }

            return Ok(cryptoRate);
        }

        [HttpGet("history-crypto-rate")]
        public async Task<ActionResult<IEnumerable<HistoryForexRate>>> GetHistoryCryptoRateAsync(
            [FromQuery] string baseCurrency, 
            [FromQuery] string quoteCurrency, 
            [FromQuery] string startDate, 
            [FromQuery] string endDate, 
            [FromQuery] string candleSize) // min, hour, day
        {
            var historyCryptoRate = await _currencyPairService.GetHistoryCryptoRate(baseCurrency, quoteCurrency, startDate, endDate, candleSize);
            if (historyCryptoRate == null)
            {
                return NotFound("Crypto rate not found.");
            }

            return Ok(historyCryptoRate);
        }

        [HttpGet("stock-rate")]
        public async Task<ActionResult<IEnumerable<StockRate>>> GetAtMarketStockRateAsync(
            [FromQuery] string stock)
        {
            var stockRate = await _currencyPairService.GetAtMarketStockRate(stock);
            if (stockRate == null)
            {
                return NotFound("Stock rate not found.");
            }

            return Ok(stockRate);
        }

        [HttpGet("history-stock-rate")]
        public async Task<ActionResult<IEnumerable<HistoryStockRate>>> GetHistoryStockRateAsync(
            [FromQuery] string stock, 
            [FromQuery] string startDate, 
            [FromQuery] string endDate, 
            [FromQuery] string candleSize) // min, hour
        {
            var historyStockRate = await _currencyPairService.GetHistoryStockRate(stock, startDate, endDate, candleSize);
            if (historyStockRate == null)
            {
                return NotFound("Stock rate not found.");
            }

            return Ok(historyStockRate);
        }
    }
}
