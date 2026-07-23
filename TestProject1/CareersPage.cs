using OpenQA.Selenium;
using System;
using System.Linq;

namespace TestingTasks
{
    public class CareersPage : BasePage
    {
        private readonly By startSearchBtn = By.XPath("//div[@class='pinned-button']//a");
        private readonly By searchField = By.Name("search");
        private readonly By countryField = By.XPath("//input[contains(@aria-label, 'country')]");
        private readonly By remoteLabel = By.XPath("//span[text()='Remote']/parent::label");
        private readonly By searchButton = By.ClassName("SearchBox_button__3ImF7");
        private readonly By jobCards = By.ClassName("JobCard_panel__gTD7e");
        private readonly By jobCardExpandIcon = By.CssSelector("[data-testid='accordion-section-header-icon-container']");

        public CareersPage(IWebDriver driver) : base(driver) { }

        public CareersPage ClickStartSearch()
        {
            var button = wait.Until(d => d.FindElement(startSearchBtn));
            button.Click();
            return this;
        }

        public CareersPage EnterKeyword(string keyword)
        {
            var field = wait.Until(d => d.FindElement(searchField));
            field.Clear();
            field.SendKeys(keyword);
            return this;
        }

        public CareersPage SelectCountry(string country)
        {
            var field = driver.FindElement(countryField);
            field.Clear();
            field.SendKeys(country);
            return this;
        }

        public CareersPage SelectRemote()
        {
            var remote = driver.FindElement(remoteLabel);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", remote);
            wait.Until(d => remote.Displayed && remote.Enabled);
            remote.Click();
            return this;
        }

        public CareersPage ClickSearch()
        {
            var button = driver.FindElement(searchButton);
            button.Click();
            AcceptCookiesIfPresent();
            return this;
        }

        public string ExpandLatestJobCardContaining(string keyword)
        {
            IWebElement targetCard = null!;
            IWebElement expandButton = null!;

            wait.Until(d =>
            {
                try
                {
                    var cards = d.FindElements(jobCards);
                    if (cards.Count == 0)
                    {
                        return false;
                    }

                    targetCard = cards.Last();
                    expandButton = targetCard.FindElement(jobCardExpandIcon);
                    return expandButton.Displayed;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });

            expandButton.Click();
            wait.Until(d => targetCard.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            return targetCard.Text;
        }
    }
}