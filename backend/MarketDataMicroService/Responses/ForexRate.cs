using System.Text.Json.Serialization;

namespace MarketDataMicroService.Responses
{
    public class ForexRate
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("quoteTimestamp")]
        public DateTime QuoteTimestamp { get; set; }

        [JsonPropertyName("bidPrice")]
        public decimal BidPrice { get; set; }

        [JsonPropertyName("bidSize")]
        public decimal BidSize { get; set; }

        [JsonPropertyName("askPrice")]
        public decimal AskPrice { get; set; }

        [JsonPropertyName("askSize")]
        public decimal AskSize { get; set; }

        [JsonPropertyName("midPrice")]
        public decimal MidPrice { get; set; }
    }
}
