using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Diagnostics;
using StudySpark.Core.Generic;



namespace StudySpark.Core.BierScraper
{
    public class BiernetScraper
    {
        public static void Main(string[] args)
        {
            BiernetScraper.ScraperOptions options = new();
            BiernetScraper scraper = new BiernetScraper(options);

            //var bierInfo = scraper.BierScrape();

            //for (int i = 0; i < bierInfo.Count; i++)
            //{
            //    for (int j = 0; j < bierInfo[i].Count; j++)
            //    {
            //        Console.WriteLine(bierInfo[i][j]);
            //    }
            //}
        }

        private ScraperOptions? scraperOptions;
        private ChromeDriver? driver;

        public BiernetScraper(ScraperOptions scraperOptions)
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

        private BierSalesScraper scraper;

        public List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierScrape(string url, string brand)
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
            BiernetScraper biernetScraper = new BiernetScraper(scraperOptions);
            biernetScraper.SetupDriver();

            //LIST OF ALL THE PRODUCT NAMES AND LOWEST PRICES (ADDED TO FINAL LIST IN THE END)
            List<List<IWebElement>> productsList = new();



            //STORE IMAGES
            List<List<IWebElement>> storeImages = new();


            //GET THE RIGHT DIV, IN THIS CASE MOBILE -- EASIER ACCESS
            IWebElement mobielDiv = biernetScraper.GetElementById("verpakkingen_mobiel");

            //GET THE LIST WHERE ALL THE RELEVANT INFORMATION IS STORED -- PER PRODUCT
            ReadOnlyCollection<IWebElement> StoreInformationGlobal = mobielDiv.FindElements(By.TagName("li"));

            //GET THE PRODUCT INFORMATION (NAME AND LOWEST PRICE) -- PER PRODUCT
            ReadOnlyCollection<IWebElement> ProductInformation = mobielDiv.FindElements(By.ClassName("ppc_text_verpakking"));

            List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BeerProductSales = new();


            BierSalesScraper.ScraperOptions options = new BierSalesScraper.ScraperOptions();
            scraper = new BierSalesScraper(options);

            //ADD ALL THE PRODUCTS AND THEIR LOWEST PRICE FOUND IN THE CORRESPONDING LIST
            for (int i = 0; i < ProductInformation.Count; i++)
            {
                string link = "";
                productsList.Add(new List<IWebElement>());
                try
                {
                    productsList[i].Add(ProductInformation[i].FindElement(By.ClassName("ppc_verpakking_titel")));
                    productsList[i].Add(ProductInformation[i].FindElement(By.ClassName("BekijkBtn")));
                    link = ProductInformation[i].FindElement(By.TagName("a")).GetAttribute("href");
                    List<GenericBeerSale> sales = scraper.BierSaleScrape(link);
                    GenericBeerProduct prod = getBeerProduct(i, productsList, brand);

                    BeerProductSales.Add(new Dictionary<GenericBeerProduct, List<GenericBeerSale>>());
                    BeerProductSales[i].Add(prod , sales);
                }
                catch (NoSuchElementException ex) { }
                catch (Exception ex) { }

            }
            return BeerProductSales;
        }
        public GenericBeerProduct getBeerProduct(int i, List<List<IWebElement>> productsList, string brand)
        {
            string productname = productsList[i][0].Text;
            string lowestprice = productsList[i][1].Text;
            int brandID = getBrandId(brand);
            GenericBeerProduct product = new GenericBeerProduct(brandID, productname, lowestprice);
            return product;
        }
        public int getBrandId(string brand)
        {
            int brandID;
            switch (brand)
            {
                case "Hertog Jan":
                    brandID = 0;
                    break;
                case "Amstel":
                    brandID = 1;
                    break;
                case "Heineken":
                    brandID = 2;
                    break;
                case "Grolsch":
                    brandID = 3;
                    break;
                case "Jupiler":
                    brandID = 4;
                    break;
                default:
                    brandID = -1;
                    break;
            }
            return brandID;
        }
        public static string getStoreID(int i)
        {
            string storeID;
            if (i == 0)
            {
                storeID = "verrander";
            }
            else
            {
                storeID = "verrander" + i;
            }
            return storeID;
        }
    }
}