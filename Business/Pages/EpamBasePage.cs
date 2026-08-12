using Core.Logging;
using Core.Pages;
using OpenQA.Selenium;

namespace Business.Pages
{
    public abstract class EpamBasePage : BasePage
    {
        // Changed from Id to XPath - Id lookup didn't work reliably on every page.
        private readonly By cookieAcceptBtn =
            By.XPath("//div[@id='onetrust-button-group']/button[@id='onetrust-accept-btn-handler']");
        private readonly By cookieBanner = By.Id("onetrust-banner-sdk");

        protected EpamBasePage(IWebDriver driver) : base(driver) { }

        public void AcceptCookiesIfPresent()
        {
            var banners = Driver.FindElements(cookieAcceptBtn);
            if (banners.Count == 0 || !banners[0].Displayed) return;

            banners[0].Click();
            TafLogger.Info("Accepted cookie banner.");

            Wait.Until(d =>
            {
                var b = d.FindElements(cookieBanner);
                return b.Count == 0 || !b[0].Displayed;
            });
        }
    }
}
