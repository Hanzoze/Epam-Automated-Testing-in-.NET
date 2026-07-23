using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestingTasks
{
    public abstract class BasePage
    {
        protected IWebDriver driver;
        protected WebDriverWait wait;

        private readonly By cookieAcceptBtn = By.XPath("//div[@id='onetrust-button-group']/button[@id='onetrust-accept-btn-handler']"); // changed from Id to Xpath because Id did not work on all pages
        private readonly By cookieBanner = By.Id("onetrust-banner-sdk"); // to check if cookie banner is still present after clicking accept

        protected BasePage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        public void AcceptCookiesIfPresent()
        {
            var banners = driver.FindElements(cookieAcceptBtn);
            if (banners.Count > 0 && banners[0].Displayed)
            {
                banners[0].Click();
                wait.Until(d =>
                {
                    var b = d.FindElements(cookieBanner);
                    return b.Count == 0 || !b[0].Displayed;
                });
            }
        }

        public string NormalizeText(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            string normalized = input.Replace('\u00A0', ' ');
            return Regex.Replace(normalized, @"\s+", " ").Trim();
        }
    }
}
