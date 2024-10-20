using System.Text.Json.Serialization;

namespace MarketDataMicroService.Responses
{
    public class StockRate
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("lastSaleTimestamp")]
        public DateTime LastSaleTimestamp { get; set; }

        [JsonPropertyName("quoteTimestamp")]
        public DateTime QuoteTimestamp { get; set; }

        [JsonPropertyName("open")]
        public decimal Open { get; set; }

        [JsonPropertyName("high")]
        public decimal High { get; set; }

        [JsonPropertyName("low")]
        public decimal Low { get; set; }

        [JsonPropertyName("mid")]
        public decimal? Mid { get; set; }  // Nullable decimal

        [JsonPropertyName("tngoLast")]
        public decimal TngoLast { get; set; }

        [JsonPropertyName("last")]
        public decimal Last { get; set; }

        [JsonPropertyName("lastSize")]
        public int? LastSize { get; set; }  // Nullable int

        [JsonPropertyName("bidSize")]
        public int? BidSize { get; set; }  // Nullable int

        [JsonPropertyName("bidPrice")]
        public decimal? BidPrice { get; set; }  // Nullable decimal

        [JsonPropertyName("askPrice")]
        public decimal? AskPrice { get; set; }  // Nullable decimal

        [JsonPropertyName("askSize")]
        public int? AskSize { get; set; }  // Nullable int

        [JsonPropertyName("prevClose")]
        public decimal PrevClose { get; set; }

        [JsonPropertyName("volume")]
        public int Volume { get; set; }
    }
}
