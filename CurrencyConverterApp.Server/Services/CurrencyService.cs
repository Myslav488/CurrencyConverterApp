using CurrencyConverterApp.Server.Interfaces;
using CurrencyConverterApp.Server.Models;
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


        public async Task<IEnumerable<ConvertedRate>> GetConvertedRatesAsync(string currency1Code, string currency2Code, DateTime startDate, DateTime endDate)
        {
            // list to store converted rates
            var convertedRates = new List<ConvertedRate>();

            // handling PLNs
            bool isCurrency1Pln = currency1Code.Equals("PLN", StringComparison.OrdinalIgnoreCase);
            bool isCurrency2Pln = currency2Code.Equals("PLN", StringComparison.OrdinalIgnoreCase);

            // get rate one
            Dictionary<DateTime, decimal> rates1 = new Dictionary<DateTime, decimal>();
            if (!isCurrency1Pln)
            {
                rates1 = await GetRatesForCurrencyAndDateRange(currency1Code, startDate, endDate);
            }

            // get rate two
            Dictionary<DateTime, decimal> rates2 = new Dictionary<DateTime, decimal>();
            if (!isCurrency2Pln)
            {
                rates2 = await GetRatesForCurrencyAndDateRange(currency2Code, startDate, endDate);
            }

            // itarate dates and convert rates
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                decimal rate1ToPln = 1.0m; // default val for PLN
                decimal rate2ToPln = 1.0m; // default val for PLN

                bool hasRate1 = isCurrency1Pln || rates1.TryGetValue(date, out rate1ToPln);
                bool hasRate2 = isCurrency2Pln || rates2.TryGetValue(date, out rate2ToPln);

                // convert if both rates are fetched for a current day
                if (hasRate1 && hasRate2)
                {
                    // rate currency 1 to currency 2 = (rate currecny 1 to PLN) / (rate currecny 2 to PLN)
                    decimal calculatedRate = rate1ToPln / rate2ToPln;
                    convertedRates.Add(new ConvertedRate
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        Rate = calculatedRate
                    });
                }
                else
                {
                    _logger.LogInformation($"Skipping conversion for {date.ToShortDateString()} due to missing rates for {currency1Code} or {currency2Code}.");
                }
            }

            return convertedRates;
        }

        private async Task<Dictionary<DateTime, decimal>> GetRatesForCurrencyAndDateRange(string currencyCode, DateTime startDate, DateTime endDate)
        {
            var rates = new Dictionary<DateTime, decimal>();
            string formattedStartDate = startDate.ToString("yyyy-MM-dd");
            string formattedEndDate = endDate.ToString("yyyy-MM-dd");

            try
            {
                // the NBP API has a limit of 93 days per query. 
                // for simplicity, we assume the range does not exceed 93 days
                var requestUri = $"exchangerates/rates/a/{currencyCode}/{formattedStartDate}/{formattedEndDate}/?format=json";
                _logger.LogInformation($"Fetching NBP rates for {currencyCode} from {formattedStartDate} to {formattedEndDate}. URI: {requestUri}");

                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var nbpResponse = JsonSerializer.Deserialize<ExchangeRatesResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (nbpResponse != null && nbpResponse.Rates != null)
                {
                    foreach (var rateDetail in nbpResponse.Rates)
                    {
                        // dictionary key is date, value is rate
                        rates[rateDetail.EffectiveDate.Date] = rateDetail.Mid;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // NBP API returns 404 Not Found if there is no data for a given range
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning($"No NBP rates found for {currencyCode} in the specified date range ({formattedStartDate} to {formattedEndDate}).");
                }
                else
                {
                    _logger.LogError(ex, $"Error fetching NBP rates for {currencyCode} from {formattedStartDate} to {formattedEndDate}.");
                    throw new ApplicationException($"Could not retrieve exchange rates for {currencyCode} from NBP API.", ex);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error deserializing NBP API response for {currencyCode} rates.");
                throw new ApplicationException("Error processing NBP API response.", ex);
            }
            return rates;
        }
    }
}
