using OpenQA.Selenium;

namespace Business.Pages
{
    public class ArticlePage : EpamBasePage
    {
        private readonly By articleTitle = By.ClassName("ai-background-video__content");

        public ArticlePage(IWebDriver driver) : base(driver) { }

        public string GetArticleTitle()
        {
            var titleElement = WaitForElement(articleTitle);
            return NormalizeText(titleElement.Text);
        }
    }
}
