using OpenQA.Selenium.Chrome;
using TestingTasks;

namespace Task_3_4
{
    public class Tests
    {
        private ChromeDriver driver = null!;
        private string downloadDirectory = null!;

        [SetUp]
        public void Setup()
        {
            downloadDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Downloads");
            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }

            var options = new ChromeOptions();
            options.AddUserProfilePreference("download.default_directory", downloadDirectory);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
        }

        [TestCase("Code-Of-Conduct_01_26.pdf")]
        public void ValidateFileDownloadTest(string expectedFileName)
        {
            var homePage = new HomePage(driver);
            homePage.Open()
                    .DownloadCodeOfConduct();

            string expectedFilePath = Path.Combine(downloadDirectory, expectedFileName);

            Assert.That(() => File.Exists(expectedFilePath), Is.True.After(15000, 500), "File failed to download in time.");

            FileInfo fileInfo = new FileInfo(expectedFilePath);
            Assert.That(fileInfo.Length, Is.GreaterThan(0), $"The downloaded file {expectedFileName} is empty.");
        }

        [Test]
        public void ValidateCarouselArticleTitleTest()
        {
            var homePage = new HomePage(driver);

            var insightsPage = homePage.Open()
                                       .NavigateToInsights();

            insightsPage.SwipeCarousel(3);

            string carouselTitle = insightsPage.GetActiveSlideTitle();

            var articlePage = insightsPage.ClickReadMore();

            string articleTitle = articlePage.GetArticleTitle();

            Assert.That(articleTitle, Does.Contain(carouselTitle).IgnoreCase,
                $"The article title '{articleTitle}' does not contain the carousel title '{carouselTitle}'");
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();

            if (Directory.Exists(downloadDirectory))
            {
                var dirInfo = new DirectoryInfo(downloadDirectory);
                foreach (var file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
            }
        }
    }
}