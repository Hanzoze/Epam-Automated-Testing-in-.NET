using Business.Pages;
using Core.Browser;
using Reqnroll;
using NUnit.Framework;

namespace Tests.StepDefinitions
{
    [Binding]
    public class ServicesNavigationSteps
    {
        private HomePage? _homePage;
        private ServicesPage? _servicesPage;

        [BeforeScenario]
        public void Setup()
        {
            DriverManager.InitDriver(BrowserType.Chrome);

            var driver = DriverManager.Driver;
            _homePage = new HomePage(driver);
        }

        [AfterScenario]
        public void TearDown()
        {
            DriverManager.QuitDriver();
        }

        [Given(@"I open the EPAM home page")]
        public void GivenIOpenTheEPAMHomePage()
        {
            _homePage!.Open("https://www.epam.com/");
        }

        [When(@"I navigate to the Services page")]
        public void WhenINavigateToTheServicesPage()
        {
            _servicesPage = _homePage!.NavigateToServices();
        }

        [When(@"I select the ""(.*)"" service category")]
        public void WhenISelectTheServiceCategory(string category)
        {
            _servicesPage!.SelectServiceCategory(category);
        }

        [Then(@"the page title should contain ""(.*)""")]
        public void ThenThePageTitleShouldContain(string expectedTitle)
        {
            string actualTitle = _servicesPage!.GetCurrentPageTitle();
            Assert.That(actualTitle, Does.Contain(expectedTitle), $"Expected title to contain '{expectedTitle}', but got '{actualTitle}'.");
        }

        [Then(@"the 'Our Related Offerings' section should be displayed")]
        public void ThenTheOurRelatedOfferingsSectionShouldBeDisplayed()
        {
            bool isDisplayed = _servicesPage!.IsRelatedOfferingsDisplayed();
            Assert.That(isDisplayed, Is.True, "The 'Our Related Offerings' section was not displayed on the page.");
        }
    }
}