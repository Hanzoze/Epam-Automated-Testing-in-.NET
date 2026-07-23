using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestingTasks
{
    public class HomePage : BasePage
    {
        private const string Url = "https://www.epam.com/";

        private readonly By insightsLink = By.LinkText("Insights");
        private readonly By codeOfConductLink = By.XPath("//a[contains(text(), 'Code of Ethical Conduct (PDF)')]"); //for some reason at the last moment by link text stopped working so I changed to Xpath
        private readonly By careersLink = By.LinkText("Careers");
        private readonly By searchToggleButton = By.XPath("//button[.//span[contains(@class,'is-a11y-only') and text()='Search']]");
        private readonly By globalSearchInput = By.Id("new_form_search");
        private readonly By findButton = By.CssSelector("button.custom-search-button"); //instead of tag name to avoid clicking the wrong button
        private readonly By searchResultItems = By.ClassName("search-results__item");

        public HomePage(IWebDriver driver) : base(driver) { }

        public HomePage Open()
        {
            driver.Navigate().GoToUrl(Url);
            AcceptCookiesIfPresent();
            return this;
        }

        public InsightsPage NavigateToInsights()
        {
            var link = wait.Until(d => d.FindElement(insightsLink));
            link.Click();
            return new InsightsPage(driver);
        }

        public void DownloadCodeOfConduct()
        {
            var link = wait.Until(d => d.FindElement(codeOfConductLink));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", link);
            wait.Until(d => link.Displayed && link.Enabled);
            link.Click();
        }

        public CareersPage NavigateToCareers()
        {
            var link = wait.Until(d => d.FindElement(careersLink));
            link.Click();
            AcceptCookiesIfPresent();
            return new CareersPage(driver);
        }

        public HomePage OpenGlobalSearch()
        {
            var toggleButton = wait.Until(d => d.FindElement(searchToggleButton));
            toggleButton.Click();
            wait.Until(d => toggleButton.GetAttribute("aria-expanded") == "true");
            return this;
        }

        public HomePage EnterGlobalSearchKeyword(string keyword)
        {
            var input = wait.Until(d => d.FindElement(globalSearchInput));
            wait.Until(d => input.Displayed && input.Enabled);
            input.Clear();
            input.SendKeys(keyword);
            return this;
        }

        public HomePage ClickFind()
        {
            var button = driver.FindElement(findButton);
            button.Click();
            wait.Until(d => d.FindElements(searchResultItems).Count > 0);
            return this;
        }

        public IEnumerable<string> GetGlobalSearchResultTexts()
        {
            return driver.FindElements(searchResultItems).Select(item => item.Text);
        }
    }
}