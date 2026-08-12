using NUnit.Framework;
using Business.Pages;
using Core.Configuration;
using Core.Tests;

namespace Tests
{
    [TestFixture]
    public class CareersSearchTests : BaseTest
    {
        [TestCase("Java", "Poland")]
        [TestCase("Python", "Ukraine")]
        public void SearchForPosition_ShouldReturnMatchingJobCard(string searchKeyword, string country)
        {
            var homePage = new HomePage(Driver);

            string jobCardText = homePage.Open(ConfigProvider.Settings.BaseUrl)
                                          .NavigateToCareers()
                                          .ClickStartSearch()
                                          .EnterKeyword(searchKeyword)
                                          .SelectCountry(country)
                                          .SelectRemote()
                                          .ClickSearch()
                                          .ExpandLatestJobCardContaining(searchKeyword);

            Assert.That(jobCardText, Does.Contain(searchKeyword).IgnoreCase,
                $"The job card does not contain the keyword: {searchKeyword}");
        }
    }
}
