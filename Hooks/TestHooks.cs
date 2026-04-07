using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using Microsoft.Playwright;
using System;
using System.IO;
using TechTalk.SpecFlow;

namespace PlaywrightBDDFramework.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly ScenarioContext _scenarioContext;

        // Playwright objects
        private static IPlaywright _playwright;
        private static IBrowser _browser;
        private IPage _page;

        // ExtentReports objects
        private static ExtentReports _extent;
        private static ExtentSparkReporter _sparkReporter;
        private ExtentTest _scenarioTest;

        public TestHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        #region Playwright + ExtentReports Setup

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string reportDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
            Directory.CreateDirectory(reportDir);

            string reportPath = Path.Combine(reportDir, $"extent-report-{DateTime.Now:yyyyMMdd_HHmmss}.html");

            _sparkReporter = new ExtentSparkReporter(reportPath);
            _sparkReporter.Config.DocumentTitle = "Automation Test Report";
            _sparkReporter.Config.ReportName = "Regression Suite";
            _sparkReporter.Config.Theme = Theme.Standard;

            _extent = new ExtentReports();
            _extent.AttachReporter(_sparkReporter);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Initialize Playwright once
            _playwright ??= Playwright.CreateAsync().GetAwaiter().GetResult();

            // Launch browser (change BrowserTypeLaunchOptions to Chromium, Firefox, or WebKit)
            _browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, // Headful to see UI
                SlowMo = 50
            }).GetAwaiter().GetResult();

            // Create context and page
            var context = _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = null
            }).GetAwaiter().GetResult();

            _page = context.NewPageAsync().GetAwaiter().GetResult();
            _scenarioContext["Page"] = _page;

            // Create scenario in Extent report
            _scenarioTest = _extent.CreateTest(_scenarioContext.ScenarioInfo.Title);
            _scenarioContext["ExtentTest"] = _scenarioTest;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioTest != null)
            {
                if (_scenarioContext.TestError != null)
                    _scenarioTest.Fail(_scenarioContext.TestError.Message);
                else
                    _scenarioTest.Pass("Scenario passed");
            }

            _page?.CloseAsync().GetAwaiter().GetResult();
            _browser?.CloseAsync().GetAwaiter().GetResult();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _extent?.Flush();
            _playwright?.Dispose();
        }

        #endregion
    }
}