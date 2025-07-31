namespace CurrencyConverterApp.Server.Models
{
    public class ConvertedRate
    {
        public string Date { get; set; } = string.Empty;
        public decimal Rate { get; set; }
    }
}
