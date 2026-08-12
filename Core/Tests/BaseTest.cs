using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Core.Browser;
using Core.Configuration;
using Core.Logging;
using Core.Utilities;

namespace Core.Tests
{
    public abstract class BaseTest
    {
        protected string? DownloadDirectory;

        protected IWebDriver Driver => DriverManager.Driver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TafLogger.Initialize(ConfigProvider.Settings.MinLogLevel);
            TafLogger.Info($"TAF running against environment '{ConfigProvider.CurrentEnvironment}'.");
        }

        [SetUp]
        public virtual void SetUp()
        {
            TafLogger.Info($"===== Starting test: {TestContext.CurrentContext.Test.FullName} =====");

            var browserType = Enum.Parse<BrowserType>(ConfigProvider.Settings.Browser, ignoreCase: true);
            DriverManager.InitDriver(browserType, ConfigProvider.Settings.Headless, DownloadDirectory);
        }

        [TearDown]
        public virtual void TearDown()
        {
            var result = TestContext.CurrentContext.Result.Outcome.Status;

            if (result == TestStatus.Failed)
            {
                TafLogger.Error($"Test failed: {TestContext.CurrentContext.Test.FullName}. " +
                                 $"Reason: {TestContext.CurrentContext.Result.Message}");
                ScreenshotHelper.Capture(Driver, TestContext.CurrentContext.Test.Name);
            }

            TafLogger.Info($"===== Finished test: {TestContext.CurrentContext.Test.FullName}. Result: {result} =====");

            DriverManager.QuitDriver();
        }
    }
}
