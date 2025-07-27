using CurrencyConverterApp.Server.Models;

namespace CurrencyConverterApp.Server.Interfaces
{
    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetAvailableCurrenciesAsync();
    }
}
