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

        public void Load()
        {
            SetupDriver();
            WaitForPageLoad();

            HandleLogIn(true);
        }

        private void HandleLogIn(bool with2FA)
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

            if (with2FA)
            {
                twoFA.SendKeys(scraperOptions?.TwoFACode);
                subButton.Click();
            }
            else
            {
                return;
            }

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("idSIButton9")));

            subButton = driver.FindElement(By.Id("idSIButton9"));

            subButton.Click();
        }

        public bool TestLoginCredentials()
        {
            HandleLogIn(false);
            WaitForPageLoad();

            //if (CheckIfIdExists("usernameError"))
            //{
            //    return false;
            //}
            //else if (CheckIfIdExists("errorText"))
            //{
            //    return false;
            //}
            //else if (CheckIfIdExists("idTxtBx_SAOTCC_OTC"))
            //{
            //    return false;
            //}
            //else if (CheckIfIdExists("idSpan_SAOTCC_Error_OTC"))
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

            try
            {
                WaitForIdLoad("idTxtBx_SAOTCC_OTC", 15);
                return true;
            } catch
            {
                return false;
            }

        }

        public void FetchSchedule()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("activitiesupcoming-link")));

            var moveToButton = driver.FindElement(By.ClassName("activitiesupcoming-link"));
            moveToButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("dhx_cal_next_button")));

            moveToButton = driver.FindElement(By.ClassName("dhx_cal_next_button"));
            moveToButton.Click();

            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("dhx_cal_event")));

            List<IWebElement> list = driver.FindElements(By.XPath(".//div//div[contains(@class,'dhx_scale_holder')]/div[contains(@class,'cal_event')]")).ToList();
            Debug.WriteLineIf(list.Count() > 0, "Data has been found.");
            Debug.WriteLineIf(list.Count() == 0, "Data has not been found.");
            foreach (IWebElement element in list)
            {
                Console.WriteLine(element.ToString() + " heeft als data " + element.GetAttribute("class") + " = " + element.Text);
                Console.WriteLine("----------");
            }
        }
    }
}
