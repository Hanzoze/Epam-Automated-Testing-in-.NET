using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingTasks
{
    public class ArticlePage : BasePage
    {
        private readonly By articleTitle = By.ClassName("ai-background-video__content");

        public ArticlePage(IWebDriver driver) : base(driver) { }

        public string GetArticleTitle()
        {
            var titleElement = wait.Until(d => d.FindElement(articleTitle));
            return NormalizeText(titleElement.Text);
        }
    }
}
