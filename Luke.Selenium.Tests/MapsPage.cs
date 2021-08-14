using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Globalization;

namespace Luke.Selenium.Pages
{
    public class MapsPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        private By cookiesConsentFrame   = By.CssSelector(".widget-consent-frame");
        private By cookiesConsentButton  = By.CssSelector("#introAgreeButton > span > span");
        private By cookiesConsentOverlay = By.CssSelector(".widget-consent-fullscreen");
        
        private By routeButton             = By.CssSelector("#searchbox-directions");
        private By routeStartingPointField = By.CssSelector("#sb_ifc51 > input");
        private By routeDestinationField   = By.CssSelector("#sb_ifc52 > input");
        
        private By onFootRouteModeButton = By.CssSelector("div[data-travel_mode='2'] > button > img");
        private By byBikeRouteModeButton = By.CssSelector("div[data-travel_mode='1'] > button > img");
        
        private By routeSearchFirstResult         = By.CssSelector("#section-directions-trip-0");
        private By routeSearchFirstResultDuration = By.CssSelector(".section-directions-trip-duration");
        private By routeSearchFirstResultDistance = By.CssSelector(".section-directions-trip-distance");

        public MapsPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }

        public void Open()
        {
            driver.Navigate().GoToUrl("https://www.google.pl/maps/");
        }

        public void AgreeToCookies()
        {
            wait.Until(ExpectedConditions.ElementIsVisible(cookiesConsentFrame));
            driver.SwitchTo().Frame(driver.FindElement(cookiesConsentFrame));
            wait.Until(ExpectedConditions.ElementIsVisible(cookiesConsentButton)).Click();
            driver.SwitchTo().DefaultContent();
        }

        public void GetFastestRoute(string startingPoint, string destination, string travelMode = "pieszo")
        {
            driver.FindElement(routeButton).Click();
            IWebElement webElement = wait.Until(ExpectedConditions.ElementIsVisible(routeStartingPointField));
            webElement.Clear();
            webElement.SendKeys(startingPoint);
            driver.FindElement(routeDestinationField).SendKeys(destination);

            switch (travelMode)
            {
                case "na rowerze":
                    driver.FindElement(byBikeRouteModeButton).Click();
                    break;
                default:
                    driver.FindElement(onFootRouteModeButton).Click();
                    break;
            }

            wait.Until(ExpectedConditions.ElementIsVisible(routeSearchFirstResult));
        }

        public int GetFastestRouteDuration()
        {
            string firstResultDuration = driver.FindElement(routeSearchFirstResultDuration).Text;
            return int.Parse(firstResultDuration.Remove(firstResultDuration.IndexOf(" ")));
        }

        public float GetFastestRouteDistance()
        {
            string firstResultDistance = driver.FindElement(routeSearchFirstResultDistance).Text.Replace(",", ".");
            return float.Parse(firstResultDistance.Remove(firstResultDistance.IndexOf(" ")), CultureInfo.InvariantCulture);
        }
    }
}
