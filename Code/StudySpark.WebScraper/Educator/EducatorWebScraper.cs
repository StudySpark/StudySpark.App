using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StudySpark.WebScraper.Educator {
    public class EducatorWebScraper : WebScraper {

        public EducatorWebScraper(string username, string password) : this(new ScraperOptions { Username = username, Password = password }) {
        }

        public EducatorWebScraper(ScraperOptions scraperOptions) : base(scraperOptions) {
            scraperOptions.URL = "https://educator.windesheim.nl";
        }

        public void Load() {
            SetupDriver();
            WaitForPageLoad();

            HandleLogIn();
        }

        private void HandleLogIn() {
            if (GetElementById("userNameInput") == null) {
                return;
            }

            GetElementById("userNameInput").SendKeys(scraperOptions?.Username);

            GetElementById("passwordInput").SendKeys(scraperOptions?.Password);

            GetElementById("submitButton").Click();
        }

        public bool TestLoginCredentials() {
            HandleLogIn();
            WaitForPageLoad();

            if (CheckIfIdExists("errorText")) {
                return false;
            }

            if (CheckIfClassExists("educator")) {
                return true;
            }

            //try {
            //    if (!GetElementById("errorText").GetAttribute("for").Length.Equals(0)) {
            //        return false;
            //    }
            //} catch (Exception) {
            //    return false;
            //}
            //Debug.WriteLine("Testing login credentials 4");

            //try {
            //    if (GetElementsByClassName("educator").Count > 0) {
            //        return true;
            //    }
            //} catch (Exception) {
            //    return false;
            //}

            return false;

        }
    }
}
