using OpenQA.Selenium.Chrome;
using TestingTasks;

namespace Task_1_2
{
    public class Tests
    {
        private ChromeDriver driver = null!;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
        }

        [TestCase("Java", "Poland")]
        [TestCase("Python", "Ukraine")]
        public void SearchForPositionTest(string searchKeyword, string country)
        {
            var homePage = new HomePage(driver);

            string jobCardText = homePage.Open()
                                          .NavigateToCareers()
                                          .ClickStartSearch()
                                          .EnterKeyword(searchKeyword)
                                          .SelectCountry(country)
                                          .SelectRemote()
                                          .ClickSearch()
                                          .ExpandLatestJobCardContaining(searchKeyword);

            Assert.That(jobCardText.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase),
                Is.True, $"The job card does not contain the keyword: {searchKeyword}");
        }

        [TestCase("BLOCKCHAIN")]
        [TestCase("Cloud")]
        [TestCase("Automation")]
        public void ValidateGlobalSearchTest(string searchKeyword)
        {
            var homePage = new HomePage(driver);

            var resultTexts = homePage.Open()
                                       .OpenGlobalSearch()
                                       .EnterGlobalSearchKeyword(searchKeyword)
                                       .ClickFind()
                                       .GetGlobalSearchResultTexts();

            bool allItemsValid = resultTexts.All(text =>
                text.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase));

            Assert.That(allItemsValid, Is.True,
                $"Validation failed: Not all items in the search results contain the word '{searchKeyword}'");
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}