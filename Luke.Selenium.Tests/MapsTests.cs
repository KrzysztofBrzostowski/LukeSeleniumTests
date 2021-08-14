using NLog;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Luke.Selenium.Pages;
using System;

namespace Luke.Selenium.Tests
{
    [TestFixture("Chrome")]
    [TestFixture("Firefox")]
    class MapsTest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string browser;

        public MapsTest(string browser)
        {
            this.browser = browser;
        }
        
        private IWebDriver driver;

        [SetUp]
        public void SetUp()
        {
            switch (browser)
            {
                case "Firefox":
                    driver = new FirefoxDriver();
                    break;
                default:
                    driver = new ChromeDriver();
                    break;
            }

            driver.Manage().Window.Maximize();
        }

        [TestCase("plac Defilad 1, Warszawa", "Chłodna 51, Warszawa", "pieszo")]
        [TestCase("plac Defilad 1, Warszawa", "Chłodna 51, Warszawa", "na rowerze")]
        [TestCase("Chłodna 51, Warszawa", "plac Defilad 1, Warszawa", "pieszo")]
        [TestCase("Chłodna 51, Warszawa", "plac Defilad 1, Warszawa", "na rowerze")]
        public void TestCase(string staringPoint, string destination, string travelMode)
        {
            MapsPage mapsPage = new MapsPage(driver);
            mapsPage.Open();
            mapsPage.AgreeToCookies();
            mapsPage.GetFastestRoute(staringPoint, destination, travelMode);

            int durationLimit;
            
            switch (travelMode)
            {
                case "na rowerze":
                    durationLimit = 15;
                    break;
                default:
                    durationLimit = 40;
                    break;
            }
            
            Assert.Greater(durationLimit, mapsPage.GetFastestRouteDuration(),
                "Czas podróży pomiędzy punktami jest większy niż " + durationLimit + " min.");

            float distanceLimit = 3;

            Assert.Greater(distanceLimit, mapsPage.GetFastestRouteDistance(),
                "Odległość pomiędzy punktami jest wieksza niż " + distanceLimit + " km.");
        }

        [TearDown]
        public void TearDown()
        {
            string testResult = TestContext.CurrentContext.Result.Outcome.Status + " | " +
                TestContext.CurrentContext.Test.FullName + Environment.NewLine;

            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                testResult += TestContext.CurrentContext.Result.Message + Environment.NewLine;
            }

            logger.Info(testResult);
            
            driver.Quit();
        }
    }
}
