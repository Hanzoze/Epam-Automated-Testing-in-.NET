using NLog;

namespace Core.Logging
{
    public static class TafLogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static bool _initialized;
        private static readonly object Lock = new();

        public static void Initialize(string minLogLevel)
        {
            if (_initialized) return;

            lock (Lock)
            {
                if (_initialized) return;

                var level = LogLevel.FromString(minLogLevel);
                var config = LogManager.Configuration;

                if (config != null)
                {
                    foreach (var rule in config.LoggingRules)
                    {
                        rule.SetLoggingLevels(level, LogLevel.Fatal);
                    }
                    LogManager.ReconfigExistingLoggers();
                }

                _initialized = true;
            }
        }

        public static void Debug(string message) => Logger.Debug(message);
        public static void Info(string message) => Logger.Info(message);
        public static void Warn(string message) => Logger.Warn(message);
        public static void Error(string message) => Logger.Error(message);
        public static void Error(Exception ex, string message) => Logger.Error(ex, message);
        public static void Fatal(string message) => Logger.Fatal(message);
    }
}
