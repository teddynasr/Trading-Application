using System.Text.Json.Serialization;

namespace MarketDataMicroService.Responses
{
    public class CryptoRate
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonPropertyName("quoteCurrency")]
        public string QuoteCurrency { get; set; }

        [JsonPropertyName("topOfBookData")]
        public List<TopOfBookData> TopOfBookData { get; set; }
    }

    public class TopOfBookData
    {
        [JsonPropertyName("quoteTimestamp")]
        public DateTime QuoteTimestamp { get; set; }

        [JsonPropertyName("lastSaleTimestamp")]
        public DateTime LastSaleTimestamp { get; set; }

        [JsonPropertyName("bidSize")]
        public decimal BidSize { get; set; }

        [JsonPropertyName("bidPrice")]
        public decimal BidPrice { get; set; }

        [JsonPropertyName("askSize")]
        public decimal AskSize { get; set; }

        [JsonPropertyName("askPrice")]
        public decimal AskPrice { get; set; }

        [JsonPropertyName("lastSize")]
        public decimal LastSize { get; set; }

        [JsonPropertyName("lastSizeNotional")]
        public decimal LastSizeNotional { get; set; }

        [JsonPropertyName("lastPrice")]
        public decimal LastPrice { get; set; }

        [JsonPropertyName("bidExchange")]
        public string BidExchange { get; set; }

        [JsonPropertyName("askExchange")]
        public string AskExchange { get; set; }

        [JsonPropertyName("lastExchange")]
        public string LastExchange { get; set; }
    }
}
