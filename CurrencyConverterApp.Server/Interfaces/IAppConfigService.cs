using CurrencyConverterApp.Server.Models;

namespace CurrencyConverterApp.Server.Interfaces
{
    public interface IAppConfigService
    {
        AppSettings GetAppSettings();
    }
}
