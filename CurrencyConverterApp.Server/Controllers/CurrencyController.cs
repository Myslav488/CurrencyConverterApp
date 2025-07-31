using CurrencyConverterApp.Server.Interfaces;
using CurrencyConverterApp.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ICurrencyService currencyService, ILogger<CurrencyController> logger)
        {
            _currencyService = currencyService;
            _logger = logger;
        }

        [HttpGet("available")] // endpoint path /api/Currency/available
        [ProducesResponseType(typeof(IEnumerable<Currency>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableCurrencies()
        {
            try
            {
                var currencies = await _currencyService.GetAvailableCurrenciesAsync();
                return Ok(currencies);
            }
            catch (ApplicationException ex)
            {
                string message = "Application error while getting available currencies.";
                _logger.LogError(ex, message);
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            catch (Exception ex)
            {
                string message = "An unexpected error occurred while getting available currencies.";
                _logger.LogError(ex, message);
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }
        }


        /// <summary>
        /// Converts the exchange rate from currency 1 to currency 2 for a given date range.
        /// Data is retrieved from the NBP API.
        /// </summary>
        /// <param name="currency1Code">Source currency code (e.g. "USD").</param>
        /// <param name="currency2Code">Destination currency code (e.g. "EUR").</param>
        /// <param name="startDate">Start date of the range.</param>
        /// <param name="endDate">End date of the range.</param>
        /// <returns>List of ConvertedRate objects (Date, Rate).</returns>
        [HttpGet("convert")] // endpoint path /api/Currency/convert
        [ProducesResponseType(typeof(IEnumerable<ConvertedRate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConvertedRates(
            [FromQuery] string currency1Code,
            [FromQuery] string currency2Code,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(currency1Code) || string.IsNullOrWhiteSpace(currency2Code))
            {
                return BadRequest("Currency codes are required.");
            }
            if (startDate > endDate)
            {
                return BadRequest("The start date cannot be later than the end date.");
            }
            if (currency1Code.Equals(currency2Code, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Please select two different currencies.");
            }

            try
            {
                var convertedRates = await _currencyService.GetConvertedRatesAsync(currency1Code, currency2Code, startDate, endDate);
                return Ok(convertedRates);
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Application error while converting currencies.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while converting currencies.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while converting currencies.");
            }
        }
    }
}
