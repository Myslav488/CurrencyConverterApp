using CurrencyConverterApp.Server.Models;

namespace CurrencyConverterApp.Server.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetAvailableCurrenciesAsync();
        Task<IEnumerable<ConvertedRate>> GetConvertedRatesAsync(string currency1Code, string currency2Code, DateTime startDate, DateTime endDate);
    }
}
