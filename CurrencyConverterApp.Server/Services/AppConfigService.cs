using CurrencyConverterApp.Server.Interfaces;
using CurrencyConverterApp.Server.Models;

namespace CurrencyConverterApp.Server.Services
{
    public class AppConfigService : IAppConfigService
    {
        private readonly AppSettings _appSettings;

        public AppConfigService(IConfiguration configuration)
        {
            _appSettings = new AppSettings();
            configuration.Bind("AppSettings", _appSettings);
        }

        public AppSettings GetAppSettings()
        {
            return _appSettings;
        }
    }
}
