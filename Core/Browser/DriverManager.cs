using Core.Logging;
using OpenQA.Selenium;

namespace Core.Browser
{
    public sealed class DriverManager
    {
        private static readonly ThreadLocal<IWebDriver?> ThreadDriver = new();

        private DriverManager() { }

        public static IWebDriver Driver
        {
            get
            {
                if (ThreadDriver.Value == null)
                    throw new InvalidOperationException(
                        "WebDriver has not been initialized for this test. Call DriverManager.InitDriver() first.");

                return ThreadDriver.Value;
            }
        }

        public static void InitDriver(BrowserType browserType, bool headless = false, string? downloadDirectory = null)
        {
            if (ThreadDriver.Value != null)
            {
                TafLogger.Warn("A driver already exists for this thread. Quitting it before creating a new one.");
                QuitDriver();
            }

            ThreadDriver.Value = BrowserFactory.CreateDriver(browserType, headless, downloadDirectory);
        }

        public static void QuitDriver()
        {
            if (ThreadDriver.Value == null) return;

            try
            {
                ThreadDriver.Value.Quit();
            }
            finally
            {
                ThreadDriver.Value.Dispose();
                ThreadDriver.Value = null;
                TafLogger.Info("Driver quit and disposed.");
            }
        }
    }
}
