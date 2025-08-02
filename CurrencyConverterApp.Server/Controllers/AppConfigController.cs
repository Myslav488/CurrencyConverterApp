using CurrencyConverterApp.Server.Interfaces;
using CurrencyConverterApp.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppConfigController : ControllerBase
    {
        private readonly IAppConfigService _appConfigService;

        public AppConfigController(IAppConfigService appConfigService)
        {
            _appConfigService = appConfigService;
        }

        [HttpGet]
        public ActionResult<AppSettings> GetAppConfig()
        {
            var config = _appConfigService.GetAppSettings();
            if (config == null)
            {
                return NotFound("Application configuration not found.");
            }
            return Ok(config);
        }
    }
}
