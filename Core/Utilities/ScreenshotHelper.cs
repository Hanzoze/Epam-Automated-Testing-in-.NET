using OpenQA.Selenium;
using Core.Logging;

namespace Core.Utilities
{
    public static class ScreenshotHelper
    {
        public static string Capture(IWebDriver driver, string testName)
        {
            try
            {
                var screenshotsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
                Directory.CreateDirectory(screenshotsDir);

                var safeTestName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
                var fileName = $"{safeTestName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
                var filePath = Path.Combine(screenshotsDir, fileName);

                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(filePath);

                TafLogger.Info($"Screenshot captured for failed test '{testName}': {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                TafLogger.Error(ex, $"Failed to capture screenshot for test '{testName}'.");
                return string.Empty;
            }
        }
    }
}
