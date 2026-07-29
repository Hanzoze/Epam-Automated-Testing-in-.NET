using OpenQA.Selenium;
using Core.Logging;

namespace Business.Pages
{
    public class CareersPage : EpamBasePage
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
            ClickElement(startSearchBtn);
            return this;
        }

        public CareersPage EnterKeyword(string keyword)
        {
            var field = WaitForElement(searchField);
            field.Clear();
            field.SendKeys(keyword);
            TafLogger.Info($"Entered keyword: {keyword}");
            return this;
        }

        public CareersPage SelectCountry(string country)
        {
            var field = Driver.FindElement(countryField);
            field.Clear();
            field.SendKeys(country);
            TafLogger.Info($"Selected country: {country}");
            return this;
        }

        public CareersPage SelectRemote()
        {
            var remote = Driver.FindElement(remoteLabel);
            ScrollIntoView(remote);
            Wait.Until(d => remote.Displayed && remote.Enabled);
            remote.Click();
            return this;
        }

        public CareersPage ClickSearch()
        {
            var button = Driver.FindElement(searchButton);
            button.Click();
            AcceptCookiesIfPresent();
            return this;
        }

        public string ExpandLatestJobCardContaining(string keyword)
        {
            IWebElement targetCard = null!;
            IWebElement expandButton = null!;

            Wait.Until(d =>
            {
                try
                {
                    var cards = d.FindElements(jobCards);
                    if (cards.Count == 0) return false;

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
            Wait.Until(d => targetCard.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            return targetCard.Text;
        }
    }
}
