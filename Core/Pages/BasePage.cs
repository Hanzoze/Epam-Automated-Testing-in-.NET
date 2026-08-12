using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Core.Configuration;
using Core.Logging;

namespace Core.Pages
{
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(ConfigProvider.Settings.DefaultWaitTimeoutSeconds));
        }

        protected IWebElement WaitForElement(By locator)
        {
            return Wait.Until(d => d.FindElement(locator));
        }

        protected void ClickElement(By locator)
        {
            var element = WaitForElement(locator);
            TafLogger.Debug($"Clicking element located by: {locator}");
            element.Click();
        }

        protected void ScrollIntoView(IWebElement element)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);
        }

        public static string NormalizeText(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var normalized = input.Replace('\u00A0', ' ');
            return Regex.Replace(normalized, @"\s+", " ").Trim();
        }
    }
}
