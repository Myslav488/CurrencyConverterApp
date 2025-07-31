namespace CurrencyConverterApp.Server.Models
{
    public class ExchangeRatesResponse
    {
        public string Table { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public List<RateDetail> Rates { get; set; } = new List<RateDetail>();
    }
}
