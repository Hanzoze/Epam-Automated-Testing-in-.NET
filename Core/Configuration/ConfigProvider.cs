using Microsoft.Extensions.Configuration;

namespace Core.Configuration
{
    public static class ConfigProvider
    {
        private static TafSettings? _settings;
        private static readonly object Lock = new();

        public static TafSettings Settings
        {
            get
            {
                if (_settings != null) return _settings;
                lock (Lock)
                {
                    _settings ??= Load();
                }
                return _settings;
            }
        }

        public static string CurrentEnvironment =>
            System.Environment.GetEnvironmentVariable("TAF_ENV") ?? "qa";

        private static TafSettings Load()
        {
            var env = CurrentEnvironment;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(prefix: "TAF_")
                .AddEnvironmentVariables()
                .Build();

            var settings = new TafSettings();
            configuration.GetSection("TafSettings").Bind(settings);

            string? pipelineBrowser = System.Environment.GetEnvironmentVariable("BROWSER");
            if (!string.IsNullOrEmpty(pipelineBrowser))
            {
                var property = typeof(TafSettings).GetProperty("BrowserType");
                if (property != null && Enum.TryParse(property.PropertyType, pipelineBrowser, true, out var parsedBrowser))
                {
                    property.SetValue(settings, parsedBrowser);
                }
            }

            return settings;
        }
    }
}
