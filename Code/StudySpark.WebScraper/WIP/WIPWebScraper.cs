using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.WebScraper.WIP
{
    public class WIPWebScraper : WebScraper
    {

        public WIPWebScraper(ScraperOptions scraperOptions) : base(scraperOptions)
        {
            scraperOptions.URL = "https://wip.windesheim.nl";
        }

        public void Load(string authCode)
        {
            SetupDriver();
            WaitForPageLoad();

            HandleLogIn();
        }

        private void HandleLogIn()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("i0116")));

            var email = driver.FindElement(By.Id("i0116"));
            var subButton = driver.FindElement(By.Id("idSIButton9"));

            email.SendKeys(scraperOptions?.Username);
            subButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("passwordInput")));

            var password = driver.FindElement(By.Id("passwordInput"));
            subButton = driver.FindElement(By.Id("submitButton"));

            password.SendKeys(scraperOptions?.Password);
            subButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("idTxtBx_SAOTCC_OTC")));

            var twoFA = driver.FindElement(By.Id("idTxtBx_SAOTCC_OTC"));
            subButton = driver.FindElement(By.Id("idSubmit_SAOTCC_Continue"));

            twoFA.SendKeys(scraperOptions?.TwoFACode);
            subButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("idSIButton9")));

            subButton = driver.FindElement(By.Id("idSIButton9"));

            subButton.Click();
        }

        public bool TestLoginCredentials(string authCode)
        {
            HandleLogIn(authCode);
            WaitForPageLoad();

            if (CheckIfIdExists("usernameError"))
            {
                Debug.WriteLine("False - Username");
                return false;
            }
            else if (CheckIfIdExists("errorText"))
            {
                Debug.WriteLine("False - Password");
                return false;
            }
            else if (CheckIfIdExists("idSpan_SAOTCC_Error_OTC"))
            {
                Debug.WriteLine("False - 2FA");
                return false;
            }
            else
            {
                Debug.WriteLine("True");
                return true;
            }

            Debug.WriteLine("False - Other");
            return false;
        }
    }
}
