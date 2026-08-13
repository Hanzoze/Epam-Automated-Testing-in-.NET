using Business.Pages;
using Core.Logging;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace Business.Pages
{
    public class HomePage : EpamBasePage
    {
        private readonly By insightsLink = By.LinkText("Insights");

        // For some reason link-text stopped working at the last moment - switched to XPath.
        private readonly By codeOfConductLink = By.XPath("//a[contains(text(), 'Code of Ethical Conduct (PDF)')]");
        private readonly By careersLink = By.LinkText("Careers");
        private readonly By searchToggleButton = By.XPath("//button[.//span[contains(@class,'is-a11y-only') and text()='Search']]");
        private readonly By globalSearchInput = By.Id("new_form_search");
        private readonly By servicesLink = By.LinkText("Services");

        // CSS by class instead of tag name, to avoid clicking the wrong button.
        private readonly By findButton = By.CssSelector("button.custom-search-button");
        private readonly By searchResultItems = By.ClassName("search-results__item");

        public HomePage(IWebDriver driver) : base(driver) { }

        public HomePage Open(string baseUrl)
        {
            TafLogger.Info($"Navigating to {baseUrl}");
            Driver.Navigate().GoToUrl(baseUrl);
            AcceptCookiesIfPresent();
            return this;
        }

        public ServicesPage NavigateToServices()
        {
            Driver.Manage().Window.Maximize();
            var link = WaitForElement(servicesLink);
            TafLogger.Info("Navigating to Services page.");
            link.Click();
            AcceptCookiesIfPresent();
            return new ServicesPage(Driver);
        }

        public InsightsPage NavigateToInsights()
        {
            ClickElement(insightsLink);
            return new InsightsPage(Driver);
        }

        public void DownloadCodeOfConduct()
        {
            var link = WaitForElement(codeOfConductLink);
            ScrollIntoView(link);
            Wait.Until(d => link.Displayed && link.Enabled);
            TafLogger.Info("Downloading Code of Ethical Conduct PDF.");
            link.Click();
        }

        public CareersPage NavigateToCareers()
        {
            ClickElement(careersLink);
            AcceptCookiesIfPresent();
            return new CareersPage(Driver);
        }

        public HomePage OpenGlobalSearch()
        {
            var toggleButton = WaitForElement(searchToggleButton);
            toggleButton.Click();
            Wait.Until(d => toggleButton.GetAttribute("aria-expanded") == "true");
            return this;
        }

        public HomePage EnterGlobalSearchKeyword(string keyword)
        {
            var input = WaitForElement(globalSearchInput);
            Wait.Until(d => input.Displayed && input.Enabled);
            input.Clear();
            input.SendKeys(keyword);
            TafLogger.Info($"Entered global search keyword: {keyword}");
            return this;
        }

        public HomePage ClickFind()
        {
            var button = Driver.FindElement(findButton);
            button.Click();
            Wait.Until(d => d.FindElements(searchResultItems).Count > 0);
            return this;
        }

        public IEnumerable<string> GetGlobalSearchResultTexts()
        {
            return Driver.FindElements(searchResultItems).Select(item => item.Text);
        }
    }
}
