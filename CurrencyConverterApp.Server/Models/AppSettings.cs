namespace CurrencyConverterApp.Server.Models
{
    public class AppSettings
    {
        public ApiSettings Api { get; set; }
        public CurrencySortingSettings CurrencySorting { get; set; }
        public LoggingSettings Logging { get; set; }
        public string DateFormat { get; set; }
    }

    public class ApiSettings
    {
        public string BaseUrl { get; set; }
    }

    public class CurrencySortingSettings
    {
        public SortBy SortBy { get; set; }
        public SortOrder SortOrder { get; set; }
    }

    public class LoggingSettings
    {
        public int LogLevel { get; set; }
    }
    public enum SortOrder
    {
        ASC,
        DESC
    }

    public enum SortBy
    {
        code,
        name
    }
}
