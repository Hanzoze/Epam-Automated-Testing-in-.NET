using Core.Logging;
using OpenQA.Selenium;

namespace Business.Pages
{
    public class ServicesPage : EpamBasePage
    {
        private readonly By visiblePageHeader = By.XPath("//*[contains(@class, 'text-ui-23')]//*[contains(@class, 'font-size-65') or self::h1]");

        private readonly By relatedOfferingsSection = By.XPath("//*[contains(text(), 'Our Related Offerings')]");

        public ServicesPage(IWebDriver driver) : base(driver) { }

        public void SelectServiceCategory(string categoryName)
        {
            By categoryLocator = By.XPath($"//a[contains(., '{categoryName}')]");

            var categoryLink = WaitForElement(categoryLocator);
            ScrollIntoView(categoryLink);

            TafLogger.Info($"Selecting service category: {categoryName}");
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", categoryLink);
        }

        public string GetCurrentPageTitle()
        {
            var titleElement = WaitForElement(visiblePageHeader);
            return titleElement.Text.Trim();
        }

        public bool IsRelatedOfferingsDisplayed()
        {
            try
            {
                var section = WaitForElement(relatedOfferingsSection);
                return section.Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                TafLogger.Info("Related Offerings section was not found within the timeout period.");
                return false;
            }
        }
    }
}