using OpenQA.Selenium;
using Core.Logging;
using System;

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
            AcceptCookiesIfPresent();

            var btn = WaitForElement(startSearchBtn);
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
            var field = WaitForElement(countryField);
            field.Clear();
            field.SendKeys(country);
            TafLogger.Info($"Selected country: {country}");
            return this;
        }

        public CareersPage SelectRemote()
        {
            var remote = WaitForElement(remoteLabel);
            ScrollIntoView(remote);
            Wait.Until(d => remote.Displayed && remote.Enabled);
            remote.Click();
            return this;
        }

        public CareersPage ClickSearch()
        {
            AcceptCookiesIfPresent();

            var button = WaitForElement(searchButton);
            ScrollIntoView(button);

            button.Click();
            return this;
        }

        public string ExpandLatestJobCardContaining(string keyword)
        {
            string extractedCardText = string.Empty;

            Wait.Until(d =>
            {
                try
                {
                    var cards = d.FindElements(jobCards);
                    if (cards.Count == 0) return false;

                    var targetCard = cards.Last();
                    var expandButton = targetCard.FindElement(jobCardExpandIcon);

                    if (!expandButton.Displayed) return false;

                    ScrollIntoView(expandButton);
                    expandButton.Click();

                    Wait.Until(innerD => targetCard.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase));

                    extractedCardText = targetCard.Text;

                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    TafLogger.Info("DOM refreshed during interaction. Retrying element fetch...");
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });

            return extractedCardText;
        }
    }
}