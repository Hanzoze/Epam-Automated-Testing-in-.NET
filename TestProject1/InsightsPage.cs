using OpenQA.Selenium;

namespace TestingTasks
{
    public class InsightsPage : BasePage
    {
        private readonly By rightArrow = By.ClassName("slider__right-arrow");
        private readonly By activeSlideTitle = By.XPath("//div[contains(@class, 'active')]//p[contains(@class, 'scaling-of-text-wrapper')]");
        private readonly By activeReadMoreBtn = By.XPath("//div[contains(@class, 'active')]//div[@data-link-name='Read the Report']//a");

        public InsightsPage(IWebDriver driver) : base(driver)
        {
            AcceptCookiesIfPresent();
        }

        public InsightsPage SwipeCarousel(int times)
        {
            var arrow = wait.Until(d => d.FindElement(rightArrow));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", arrow);

            for (int i = 0; i < times; i++)
            {
                wait.Until(d => arrow.Displayed && arrow.Enabled);
                arrow.Click();
                Thread.Sleep(1000);
            }
            return this;
        }

        public string GetActiveSlideTitle()
        {
            var titleElement = wait.Until(d => d.FindElement(activeSlideTitle));
            return NormalizeText(titleElement.Text);
        }

        public ArticlePage ClickReadMore()
        {
            var btn = driver.FindElement(activeReadMoreBtn);
            btn.Click();
            return new ArticlePage(driver);
        }
    }
}
