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
    }
}
