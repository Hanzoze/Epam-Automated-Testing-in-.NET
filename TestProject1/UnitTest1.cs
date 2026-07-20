using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace TestProject1
{
    public class Tests
    {
        private ChromeDriver driver = null!;
        private WebDriverWait wait = null!;

        private const string BaseUrl = "https://www.epam.com/";
        private const string CookieAcceptXPath = "//div[@id='onetrust-button-group']/button[@id='onetrust-accept-btn-handler']";
        private const string CookieBannerId = "onetrust-banner-sdk";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            options.AcceptInsecureCertificates = true;

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private void AcceptCookiesIfPresent()
        {
            var banners = driver.FindElements(By.XPath(CookieAcceptXPath));
            if (banners.Count > 0 && banners[0].Displayed)
            {
                banners[0].Click();
                wait.Until(d =>
                {
                    var b = d.FindElements(By.Id(CookieBannerId));
                    return b.Count == 0 || !b[0].Displayed;
                });
            }
        }

        private IWebElement WaitUntilClickable(IWebElement element)
        {
            wait.Until(d => element.Displayed && element.Enabled);
            return element;
        }

        private void ClickReliably(IWebElement element)
        {
            WaitUntilClickable(element);
            try
            {
                element.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
            catch (ElementNotInteractableException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
        }

        private IWebElement OpenGlobalSearch()
        {
            IWebElement toggleButton = wait.Until(d =>
                d.FindElement(By.XPath("//button[.//span[contains(@class,'is-a11y-only') and text()='Search']]")));

            ClickReliably(toggleButton);

            wait.Until(d => toggleButton.GetAttribute("aria-expanded") == "true");

            IWebElement searchInput = wait.Until(d => d.FindElement(By.Id("new_form_search")));
            WaitUntilClickable(searchInput);

            return searchInput;
        }

        private IWebElement ExpandLatestJobCardContaining(string searchKeyword)
        {
            IWebElement targetJobCard = null;
            IWebElement expandButton = null;

            wait.Until(d =>
            {
                try
                {
                    var jobCards = d.FindElements(By.ClassName("JobCard_panel__gTD7e"));
                    if (jobCards.Count == 0)
                    {
                        return false;
                    }

                    targetJobCard = jobCards.Last();
                    expandButton = targetJobCard.FindElement(By.CssSelector("[data-testid='accordion-section-header-icon-container']"));
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
            wait.Until(d => targetJobCard.Text.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase));

            return targetJobCard!;
        }

        [TestCase("Java", "Poland")]
        [TestCase("Python", "Ukraine")]
        public void SearchForPositionTest(string searchKeyword, string country)
        {
            driver.Navigate().GoToUrl(BaseUrl);
            AcceptCookiesIfPresent();
            IWebElement careersLink = wait.Until(d => d.FindElement(By.LinkText("Careers")));

            careersLink.Click();
            AcceptCookiesIfPresent();
            IWebElement startSearchBtn = wait.Until(d => d.FindElement(By.XPath("//div[@class='pinned-button']//a")));
            startSearchBtn.Click();

            IWebElement searchField = wait.Until(d => d.FindElement(By.Name("search")));
            searchField.Clear();
            searchField.SendKeys(searchKeyword);

            IWebElement dropDownCountries = driver.FindElement(By.XPath("//input[contains(@aria-label, 'country')]"));
            dropDownCountries.Clear();
            dropDownCountries.SendKeys(country);

            IWebElement comboBoxRemote = driver.FindElement(By.XPath("//span[text()='Remote']/parent::label"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", comboBoxRemote);
            ClickReliably(comboBoxRemote);

            IWebElement searchButton = driver.FindElement(By.ClassName("SearchBox_button__3ImF7"));
            searchButton.Click();
            AcceptCookiesIfPresent();

            IWebElement resultJobCard = ExpandLatestJobCardContaining(searchKeyword);

            Assert.That(resultJobCard.Text.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase),
                Is.True, $"The job card does not contain the keyword: {searchKeyword}");
        }

        [TestCase("BLOCKCHAIN")]
        [TestCase("Cloud")]
        [TestCase("Automation")]
        public void ValidateGlobalSearchTest(string searchKeyword)
        {
            driver.Navigate().GoToUrl(BaseUrl);
            AcceptCookiesIfPresent();

            IWebElement searchString = OpenGlobalSearch();
            searchString.Clear();
            searchString.SendKeys(searchKeyword);

            IWebElement findButton = driver.FindElement(By.CssSelector("button.custom-search-button"));
            findButton.Click();
            wait.Until(d => d.FindElements(By.ClassName("search-results__item")).Count > 0);

            var searchResultsItems = driver.FindElements(By.ClassName("search-results__item"));
            bool allItemsValid = searchResultsItems.All(item =>
                item.Text.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase));

            Assert.That(allItemsValid, Is.True, $"Validation failed: Not all items in the search results contain the word '{searchKeyword}'");
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}