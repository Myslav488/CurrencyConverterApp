namespace CurrencyConverterApp.Server.Models
{
    public class Rate
    {
        public string Currency { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public decimal Mid { get; set; }
    }
}
