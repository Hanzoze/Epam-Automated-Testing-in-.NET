using NUnit.Framework;
using Business.Pages;
using Core.Configuration;
using Core.Tests;

namespace Tests
{
    [TestFixture]
    public class FileDownloadTests : BaseTest
    {
        [SetUp]
        public override void SetUp()
        {
            DownloadDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Downloads");
            Directory.CreateDirectory(DownloadDirectory);

            base.SetUp();
        }

        [TestCase("Code-Of-Conduct_01_26.pdf")]
        public void ValidateFileDownload_ShouldDownloadNonEmptyPdf(string expectedFileName)
        {
            var homePage = new HomePage(Driver);
            homePage.Open(ConfigProvider.Settings.BaseUrl)
                    .DownloadCodeOfConduct();

            string expectedFilePath = Path.Combine(DownloadDirectory!, expectedFileName);

            Assert.That(() => File.Exists(expectedFilePath), Is.True.After(15000, 500),
                "File failed to download in time.");

            var fileInfo = new FileInfo(expectedFilePath);
            Assert.That(fileInfo.Length, Is.GreaterThan(0),
                $"The downloaded file {expectedFileName} is empty.");
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            if (DownloadDirectory != null && Directory.Exists(DownloadDirectory))
            {
                foreach (var file in new DirectoryInfo(DownloadDirectory).GetFiles())
                {
                    file.Delete();
                }
            }
        }
    }
}
