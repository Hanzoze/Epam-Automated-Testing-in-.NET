using Business.Pages;
using Core.Configuration;
using Core.Tests;

namespace Tests
{
    [TestFixture]
    public class GlobalSearchTests : BaseTest
    {
        [TestCase("BLOCKCHAIN")]
        [TestCase("Cloud")]
        [TestCase("Automation")]
        public void ValidateGlobalSearch_ShouldReturnResultsContainingKeyword(string searchKeyword)
        {
            var homePage = new HomePage(Driver);

            var resultTexts = homePage.Open(ConfigProvider.Settings.BaseUrl)
                                       .OpenGlobalSearch()
                                       .EnterGlobalSearchKeyword(searchKeyword)
                                       .ClickFind()
                                       .GetGlobalSearchResultTexts();

            bool allItemsValid = resultTexts.All(text =>
                text.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase));

            Assert.That(allItemsValid, Is.True,
                $"Validation failed: Not all items in the search results contain the word '{searchKeyword}'");
        }
    }
}
