using NUnit.Framework;
using Business.Pages;
using Core.Configuration;
using Core.Tests;

namespace Tests
{
    [TestFixture]
    public class CarouselArticleTests : BaseTest
    {
        [Test]
        public void ValidateCarouselArticleTitle_ShouldMatchArticlePageTitle()
        {
            var homePage = new HomePage(Driver);

            var insightsPage = homePage.Open(ConfigProvider.Settings.BaseUrl)
                                        .NavigateToInsights();

            insightsPage.SwipeCarousel(3);

            string carouselTitle = insightsPage.GetActiveSlideTitle();

            var articlePage = insightsPage.ClickReadMore();
            string articleTitle = articlePage.GetArticleTitle();

            Assert.That(articleTitle, Does.Contain(carouselTitle).IgnoreCase,
                $"The article title '{articleTitle}' does not contain the carousel title '{carouselTitle}'");
        }
    }
}
