using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //LIST OF ALL THE SALES PER PRODUCT (ALSO ADDED TO FINAL LIST IN THE END)
        public List<Dictionary<IWebElement, IWebElement>> salesList = new();

        //STORE IMAGES
        public List<List<IWebElement>> storeImages = new();

        public void BierSaleScrape(string url)
        {
            //CREATE LIST THAT IS BEING RETURNED 
            List<List<object>> BierInformatie = new();

            //VARIABLE NAMES FOR INDECES IN PRODUCTSLIST (NOT FINAL LIST)
            int PRODUCT_NAME = 0;
            int PRODUCT_LOWEST_PRICE = 1;

            //MAKE SCRAPEROPTIONS (ONLY URL IN THIS CASE)
            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.URL = url;

            //CREATE ACTUAL SCRAPER
            BierSalesScraper biernetScraper = new BierSalesScraper(scraperOptions);
            biernetScraper.SetupDriver();


            //GET THE RIGHT DIV, IN THIS CASE ajaxdiv -- EASIER ACCESS
            IWebElement ajaxdiv = biernetScraper.GetElementById("inhoud_ajaxdiv");

            //Get aanbiedingen div
            IWebElement aanbiedingenDiv = ajaxdiv.FindElement(By.ClassName("aanbiedingen"));

            //GET THE LIST of all sales
            ReadOnlyCollection<IWebElement> StoreInformationGlobal = aanbiedingenDiv.FindElements(By.ClassName("cardStyle"));

            //LOOP THROUGH ALL THE AVAILABLE SALES -- PER PRODUCT
            for (int i = 0; i < StoreInformationGlobal.Count; i++)
            {
                //GET ALL THE STORES WITH A SALE BASED ON WHICH PRODUCT IT IS LOOKING AT
                IWebElement winkelsInformation = StoreInformationGlobal[i].FindElement(By.ClassName("informatie"));
                ReadOnlyCollection<IWebElement> prijsinfo;
                try
                {
                    prijsinfo = winkelsInformation.FindElements(By.ClassName("prijss"));
                } catch (NoSuchElementException ex)
                {
                    prijsinfo = winkelsInformation.FindElements(By.ClassName("prijsss"));
                }
                

                //ADD AN EMPTY DICTIONARY TO SALESLIST -- WE PUT THE SALE IN THIS DICTIONARY
                //FOR EVERY SALE, A DICTIONARY IS CREATED
                salesList.Add(new Dictionary<IWebElement, IWebElement>());
                storeImages.Add(new List<IWebElement>());
                for (int j = 0; j < prijsinfo.Count; j++)
                {
                    //IF THERE IS A SALE AVAILABLE -- ADD IT
                    try
                    {
                        salesList[i].Add(prijsinfo[j].FindElement(By.ClassName("van_prijss")), prijsinfo[j].FindElement(By.ClassName("voor_prijsss")));
                    }
                    //ELSE DON'T DO ANYTHING
                    catch (Exception e) { }
                    try
                    {
                        storeImages[i].Add(StoreInformationGlobal[j].FindElement(By.ClassName("lazyloading")));
                    }
                    catch (Exception e) { }


                }
            }
        }

        public List<Dictionary<IWebElement, IWebElement>> GetSalesList()
        {
            return salesList;
        }
        public List<List<IWebElement>> getStoreImages()
        {
            return storeImages;
        }
    }
}
