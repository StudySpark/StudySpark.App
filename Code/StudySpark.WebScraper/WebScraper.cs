﻿using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;
using System.Threading;

namespace StudySpark.WebScraper {
    public class WebScraper {

        internal ScraperOptions scraperOptions;
        internal ChromeDriver? driver;

        public WebScraper(ScraperOptions scraperOptions) {
            this.scraperOptions = scraperOptions;
        }

        public void SetupDriver(bool mobileView = false) {
            var options = new ChromeOptions();
            if (!scraperOptions.Debug) {
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
            }

            string dataDirPath = scraperOptions.DataDirPath;
            dataDirPath = dataDirPath.EndsWith("/") ? dataDirPath.Remove(dataDirPath.Length - 1, 1) : dataDirPath;
            if (dataDirPath.Length > 0) {
                dataDirPath = Path.GetFullPath(dataDirPath);
                dataDirPath = Path.Combine(dataDirPath, "chrome-session");
                options.AddArgument($"user-data-dir={dataDirPath}");
                options.AddArgument("--profile-directory=Default");
            }

            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            driver = new ChromeDriver(chromeDriverService, options);

            if(mobileView)
            {
                driver.Manage().Window.Size = new System.Drawing.Size(100, 800);
            }
            driver?.Navigate().GoToUrl(scraperOptions?.URL);
        }

        public void CloseDriver() {            
            if (driver == null) {
                return;
            }

            driver.Close();
            driver.Quit();
        }

        public void WaitForPageLoad(uint timeout = 30) {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(ExpectedConditions.ElementExists(By.TagName("body")));
        }

        public void WaitForIdLoad(string id, uint timeout = 30) {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(ExpectedConditions.ElementExists(By.Id(id)));
        }

        public IWebElement GetElementById(string element, uint timeout = 30) {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));

            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(element)));
        }

        public ReadOnlyCollection<IWebElement> GetElementsByClassName(string element, uint timeout = 30) {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));

            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName(element)));
        }

        public bool CheckIfIdExists(string element, uint timeout = 30) {
            try {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));

                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id(element))) != null;
            } catch (NoSuchElementException) {
                try {
                    return driver?.FindElement(By.Id(element)) != null;
                } catch {
                    return false;
                }
            }
        }

        public bool CheckIfClassExists(string element) {
            return driver?.FindElements(By.ClassName(element)).Count > 0;
        }
    }
}
