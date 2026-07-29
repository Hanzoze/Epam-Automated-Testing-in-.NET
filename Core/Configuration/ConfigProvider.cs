using Microsoft.Extensions.Configuration;

namespace Core.Configuration
{
    /// <summary>
    /// Loads TAF configuration once per test run.
    /// Environment is selected via the TAF_ENV environment variable
    /// (falls back to "qa"), so the same binaries can run against
    /// QA / Staging / Prod without a rebuild.
    /// </summary>
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
                .Build();

            var settings = new TafSettings();
            configuration.GetSection("TafSettings").Bind(settings);
            return settings;
        }
    }
}
