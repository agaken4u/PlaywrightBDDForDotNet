using TechTalk.SpecFlow;
using Microsoft.Playwright;
using AventStack.ExtentReports;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightBDDFramework1
{
    [Binding]
    public class TestDataHelper
    {
        private readonly ScenarioContext _scenarioContext;

        // Example test data
        public string SampleData { get; private set; } = string.Empty;

        public TestDataHelper(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        // -------------------------------
        // SAFE GETTERS (Prevents crashes)
        // -------------------------------

        private IPage? GetPage()
        {
            _scenarioContext.TryGetValue("Page", out var page);
            return page as IPage;
        }

        private ExtentTest? GetTest()
        {
            _scenarioContext.TryGetValue("ExtentTest", out var test);
            return test as ExtentTest;
        }

        // -------------------------------
        // BEFORE EACH SCENARIO
        // -------------------------------
        [BeforeScenario]
        public void BeforeScenario()
        {
            SampleData = "Hello, Playwright!";
            TestContext.Progress.WriteLine("BeforeScenario: Test data initialized.");

            // ✅ Safe logging
            GetTest()?.Info("Test data initialized for scenario.");
        }

        // -------------------------------
        // AFTER EACH SCENARIO
        // -------------------------------
        [AfterScenario]
        public async Task AfterScenario()
        {
            SampleData = string.Empty;
            TestContext.Progress.WriteLine("AfterScenario: Test data cleared.");

            var test = GetTest();
            var page = GetPage();

            test?.Info("Test data cleared after scenario.");

            // 📸 Screenshot on failure (SAFE)
            if (_scenarioContext.TestError != null && page != null && test != null)
            {
                var scenarioName = _scenarioContext.ScenarioInfo.Title;
                var safeName = SanitizeFileName(scenarioName);

                var reportDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
                Directory.CreateDirectory(reportDir);

                var screenshotPath = Path.Combine(reportDir, $"{safeName}.png");

                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath,
                    FullPage = true
                });

                test.Fail("Scenario failed")
                    .AddScreenCaptureFromPath(screenshotPath);
            }
        }

        // -------------------------------
        // HELPER METHOD
        // -------------------------------
        public void ValidateSampleData()
        {
            Assert.IsNotNull(SampleData, "SampleData should not be null.");
            TestContext.Progress.WriteLine($"SampleData = {SampleData}");

            GetTest()?.Pass($"Validated SampleData: {SampleData}");
        }

        // -------------------------------
        // HELPER: SAFE FILE NAME
        // -------------------------------
        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            return name.Replace(" ", "_");
        }
    }
}