using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.WebScraper.WIP
{
    internal class WIPWebScraper : WebScraper
    {

        public WIPWebScraper(ScraperOptions scraperOptions) : base(scraperOptions)
        {
            scraperOptions.URL = "https://wip.windesheim.nl";
        }

        public void Load()
        {
            SetupDriver();
            WaitForPageLoad();

            bool accRegistered = CheckForAccounts();
        }

        private bool CheckForAccounts()
        {

            List<IWebElement> accButtons = driver.FindElements(By.ClassName("table")).ToList();

            foreach (IWebElement accButton in accButtons) 
            { 
                Debug.WriteLine(accButton);
            }

            if (accButtons.Count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
