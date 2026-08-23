using Core.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace Core.Browser
{
    public static class BrowserFactory
    {
        public static IWebDriver CreateDriver(BrowserType browserType, bool headless = false, string? downloadDirectory = null)
        {
            TafLogger.Info($"Creating '{browserType}' driver (headless={headless}, downloadDir={downloadDirectory ?? "n/a"}).");

            IWebDriver driver = browserType switch
            {
                BrowserType.Chrome => CreateChromeDriver(headless, downloadDirectory),
                BrowserType.Firefox => CreateFirefoxDriver(headless, downloadDirectory),
                BrowserType.Edge => CreateEdgeDriver(headless, downloadDirectory),
                _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type.")
            };

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            driver.Manage().Window.Maximize();
            return driver;
        }

        private static IWebDriver CreateChromeDriver(bool headless, string? downloadDirectory)
        {
            var options = new ChromeOptions();
            if (headless) options.AddArgument("--headless=new");
            options.AddArgument("--start-maximized");

            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-infobars");

            if (!string.IsNullOrWhiteSpace(downloadDirectory))
            {
                options.AddUserProfilePreference("download.default_directory", downloadDirectory);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            }

            return new ChromeDriver(options);
        }

        private static IWebDriver CreateFirefoxDriver(bool headless, string? downloadDirectory)
        {
            var options = new FirefoxOptions();
            if (headless) options.AddArgument("-headless");

            options.SetPreference("dom.webdriver.enabled", false);
            options.SetPreference("useAutomationExtension", false);

            if (!string.IsNullOrWhiteSpace(downloadDirectory))
            {
                options.SetPreference("browser.download.folderList", 2);
                options.SetPreference("browser.download.dir", downloadDirectory);
                options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/pdf");
                options.SetPreference("pdfjs.disabled", true);
            }

            return new FirefoxDriver(options);
        }

        private static IWebDriver CreateEdgeDriver(bool headless, string? downloadDirectory)
        {
            var options = new EdgeOptions();
            if (headless) options.AddArgument("headless");

            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--disable-blink-features=AutomationControlled");

            if (!string.IsNullOrWhiteSpace(downloadDirectory))
            {
                options.AddUserProfilePreference("download.default_directory", downloadDirectory);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            }

            return new EdgeDriver(options);
        }
    }
}