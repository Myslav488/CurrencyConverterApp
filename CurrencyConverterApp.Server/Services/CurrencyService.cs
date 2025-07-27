using CurrencyConverterApp.Server.Interfaces;
using CurrencyConverterApp.Server.Models;
using System.Net.Http;
using System.Text.Json;

namespace CurrencyConverterApp.Server.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(HttpClient httpClient, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://api.nbp.pl/api/");
        }

        public async Task<IEnumerable<Currency>> GetAvailableCurrenciesAsync()
        {
            try
            {
                // get currencies
                var response = await _httpClient.GetAsync("exchangerates/tables/A/?format=json");
                response.EnsureSuccessStatusCode(); // throws an exception if gets response code >400 or >5oo

                var jsonString = await response.Content.ReadAsStringAsync();
                var nbpResponse = JsonSerializer.Deserialize<List<ExchangeRates>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // assert not empty response
                if (nbpResponse == null || !nbpResponse.Any())
                {
                    _logger.LogWarning("NBP API returned no data for available currencies.");
                    return Enumerable.Empty<Currency>();
                }

                // add PLN as base currency
                var currencies = new List<Currency>
                {
                    new Currency { Code = "PLN", Name = "Polski Złoty" }
                };

                // map data
                currencies.AddRange(nbpResponse.First().Rates.Select(r => new Currency
                {
                    Code = r.Code,
                    Name = r.Currency
                }).OrderBy(c => c.Name));

                return currencies;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching available currencies from NBP API.");
                throw new ApplicationException("Could not retrieve available currencies from NBP API.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing API response.");
                throw new ApplicationException("Error processing API response.", ex);
            }
        }
    }
}
