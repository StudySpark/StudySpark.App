﻿using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;

namespace StudySpark.WebScraper {
    public class WebScraper {

        internal ScraperOptions scraperOptions;
        internal ChromeDriver? driver;

        public WebScraper(ScraperOptions scraperOptions) {
            this.scraperOptions = scraperOptions;
        }

        public void SetupDriver() {
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

            driver = new ChromeDriver(options);

            //LoadCookies();

            driver?.Navigate().GoToUrl(scraperOptions?.URL);
        }

        public void CloseDriver() {
            if (driver == null) {
                return;
            }

            //SaveCookies();

            driver.Close();
        }

        public void WaitForPageLoad(uint timeout = 10) {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(ExpectedConditions.ElementExists(By.TagName("body")));
        }

        public IWebElement GetElementById(string element, uint timeout = 10) {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));

            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(element)));
        }

        public ReadOnlyCollection<IWebElement> GetElementsByClassName(string element, uint timeout = 10) {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));

            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName(element)));
        }

        //private void SaveCookies() {
        //    if (scraperOptions.CookieStorageFilePath.Length == 0 || driver == null) {
        //        return;
        //    }

        //    // Get the cookies
        //    var cookies = driver.Manage().Cookies.AllCookies;

        //    // Convert cookies to serializable format
        //    var serializableCookies = cookies.Select(c => new SerializableCookie(c)).ToList();

        //    // Serialize cookies to a file (e.g., using JSON)
        //    string cookieJson = Newtonsoft.Json.JsonConvert.SerializeObject(serializableCookies);
        //    File.WriteAllText(scraperOptions.CookieStorageFilePath, cookieJson);
        //}

        //private void LoadCookies() {
        //    if (scraperOptions.CookieStorageFilePath.Length == 0 || driver == null) {
        //        return;
        //    }

        //    driver.Manage().Cookies.DeleteAllCookies();

        //    try {
        //        // Read cookies from the file
        //        string cookieJson = File.ReadAllText(scraperOptions.CookieStorageFilePath);
        //        var serializableCookies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SerializableCookie>>(cookieJson);

        //        // Add cookies to the driver
        //        if (serializableCookies != null) {
        //            foreach (var serializableCookie in serializableCookies) {
        //                driver.Manage().Cookies.AddCookie(serializableCookie.ToCookie());
        //            }
        //        }
        //    } catch (Exception ex) {
        //        // Handle exceptions (e.g., file not found, invalid JSON, etc.)
        //        Console.WriteLine($"Error loading cookies: {ex.Message}");
        //    }
        //}
    }
}
