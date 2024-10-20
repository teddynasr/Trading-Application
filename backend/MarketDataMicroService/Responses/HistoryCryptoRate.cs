using System.Text.Json.Serialization;

namespace MarketDataMicroService.Responses
{
    public class HistoryCryptoRate
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonPropertyName("quoteCurrency")]
        public string QuoteCurrency { get; set; }

        [JsonPropertyName("priceData")]
        public List<PriceData> PriceData { get; set; }
    }

    public class PriceData
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("open")]
        public decimal Open { get; set; }

        [JsonPropertyName("high")]
        public decimal High { get; set; }

        [JsonPropertyName("low")]
        public decimal Low { get; set; }

        [JsonPropertyName("close")]
        public decimal Close { get; set; }

        [JsonPropertyName("volume")]
        public decimal Volume { get; set; }

        [JsonPropertyName("volumeNotional")]
        public decimal VolumeNotional { get; set; }

        [JsonPropertyName("tradesDone")]
        public decimal TradesDone { get; set; }
    }
}
