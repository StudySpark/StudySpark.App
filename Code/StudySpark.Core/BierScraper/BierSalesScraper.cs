using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using StudySpark.Core.Generic;

namespace StudySpark.Core.BierScraper
{
    internal class BierSalesScraper
    {
        public static void Main(string[] args)
        {
            BierSalesScraper.ScraperOptions options = new();
            BierSalesScraper scraper = new BierSalesScraper(options);
        }

        private ScraperOptions? scraperOptions;
        private ChromeDriver? driver;

        public BierSalesScraper(ScraperOptions scraperOptions)
        {
            this.scraperOptions = scraperOptions;
        }
        public void SetupDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            //options.AddArgument("--disable-gpu");

            driver = new ChromeDriver(chromeDriverService, options);
            driver?.Navigate().GoToUrl(scraperOptions?.URL);
            driver.Manage().Window.Size = new System.Drawing.Size(100, 800);
        }

        public void CloseDriver()
        {
            if (driver == null)
            {
                return;
            }

            driver.Close();
        }

        public IWebElement GetElementById(string element)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(element)));
        }

        public ReadOnlyCollection<IWebElement> GetElementsByClassName(string element)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName(element)));
        }

        public ReadOnlyCollection<IWebElement> GetElementsByXPath(string XPath)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath(XPath)));
        }

        public class ScraperOptions
        {
            public string? URL { get; set; }
        }


        public List<GenericBeerSale> BierSaleScrape(string url)
        {

            List<List<object>> BierInformatie = new();
            List<GenericBeerSale> salesList = new List<GenericBeerSale>();

            int PRODUCT_NAME = 0;
            int PRODUCT_LOWEST_PRICE = 1;

            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.URL = url;

            BierSalesScraper biernetScraper = new BierSalesScraper(scraperOptions);
            biernetScraper.SetupDriver();

            IWebElement ajaxdiv = biernetScraper.GetElementById("inhoud_ajaxdiv");

            IWebElement aanbiedingenDiv = ajaxdiv.FindElement(By.ClassName("aanbiedingen"));

            ReadOnlyCollection<IWebElement> StoreInformationGlobal = aanbiedingenDiv.FindElements(By.ClassName("cardStyle"));

            string oldprice = "";
            string newprice = "";
            string store = "";
            string storeimage = "";
            string expiration_date = "";
            //LOOP THROUGH ALL THE AVAILABLE SALES -- PER PRODUCT
            for (int i = 0; i < StoreInformationGlobal.Count; i++)
            {
                
                IWebElement prijsinfo = null;

                //GET ALL THE STORES WITH A SALE BASED ON WHICH PRODUCT IT IS LOOKING AT
                IWebElement winkelsInformation = StoreInformationGlobal[i].FindElement(By.ClassName("informatie"));
                try
                {
                    prijsinfo = winkelsInformation.FindElement(By.ClassName("prijss"));
                    oldprice = prijsinfo.FindElement(By.ClassName("van_prijss")).Text;
                    newprice = prijsinfo.FindElement(By.ClassName("voor_prijss")).Text;
                }
                catch (NoSuchElementException)
                {
                    prijsinfo = winkelsInformation.FindElement(By.ClassName("prijsss"));
                    oldprice = prijsinfo.FindElement(By.ClassName("van_prijsss")).Text;
                    newprice = prijsinfo.FindElement(By.ClassName("voor_prijsss")).Text;

                    IWebElement imageDiv = StoreInformationGlobal[i].FindElement(By.ClassName("logo_image"));
                    store = imageDiv.FindElement(By.TagName("img")).GetAttribute("alt");
                    storeimage = "https://www.biernet.nl" + imageDiv.FindElement(By.TagName("img")).GetAttribute("data-src");

                    IWebElement footer = StoreInformationGlobal[i].FindElement(By.ClassName("footer-item"));
                    expiration_date = footer.FindElement(By.ClassName("laatste_regel")).Text.Replace("t/m", "");
                }

              
                salesList.Add(new GenericBeerSale(store, storeimage, oldprice, newprice, expiration_date));
            }

            return salesList;
        }
    }
}
