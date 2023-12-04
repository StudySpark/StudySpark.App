using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using OpenQA.Selenium.DevTools.V117.Accessibility;


namespace StudySpark.Core.BierScraper
{
    public class BiernetScraper
    {
        public static void Main(string[] args)
        {
            BiernetScraper.ScraperOptions options = new();
            BiernetScraper scraper = new BiernetScraper(options);

            var bierInfo = scraper.BierScrape();

            for (int i = 0; i < bierInfo.Count; i++)
            {
                for (int j = 0; j < bierInfo[i].Count; j++)
                {
                    Console.WriteLine(bierInfo[i][j]);
                }
            }
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
            //options.AddArgument("--disable-gpu");

            driver = new ChromeDriver(options);
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


        public List<List<object>> BierScrape()
        {
            //CREATE LIST THAT IS BEING RETURNED
            List<List<object>> BierInformatie = new();

            //VARIABLE NAMES FOR INDECES IN PRODUCTSLIST (NOT FINAL LIST)
            int PRODUCT_NAME = 0;
            int PRODUCT_LOWEST_PRICE = 1;
            int PRODUCT_SALES = 2;
            int PRODUCT_IMAGE = 3;

            //MAKE SCRAPEROPTIONS (ONLY URL IN THIS CASE)
            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.URL = "https://www.biernet.nl/bier/merken/hertog-jan-pilsener";

            //CREATE ACTUAL SCRAPER
            BiernetScraper biernetScraper = new BiernetScraper(scraperOptions);
            biernetScraper.SetupDriver();

            //LIST OF ALL THE PRODUCT NAMES AND LOWEST PRICES (ADDED TO FINAL LIST IN THE END)
            List<List<IWebElement>> productsList = new();

            //LIST OF ALL THE SALES PER PRODUCT (ALSO ADDED TO FINAL LIST IN THE END)
            List<Dictionary<IWebElement, IWebElement>> salesList = new();

            //GET THE RIGHT DIV, IN THIS CASE MOBILE -- EASIER ACCESS
            IWebElement mobielDiv = biernetScraper.GetElementById("verpakkingen_mobiel");

            //GET THE LIST WHERE ALL THE RELEVANT INFORMATION IS STORED -- PER PRODUCT
            ReadOnlyCollection<IWebElement> StoreInformationGlobal = mobielDiv.FindElements(By.TagName("li"));

            //GET THE PRODUCT INFORMATION (NAME AND LOWEST PRICE) -- PER PRODUCT
            ReadOnlyCollection<IWebElement> ProductInformation = mobielDiv.FindElements(By.ClassName("ppc_text_verpakking"));

            ReadOnlyCollection<IWebElement> ProductImage = mobielDiv.FindElements(By.ClassName("linkImgPPC"));



            //ADD ALL THE PRODUCTS AND THEIR LOWEST PRICE FOUND IN THE CORRESPONDING LIST
            for (int i = 0; i < ProductInformation.Count; i++)
            {
                productsList.Add(new List<IWebElement>());
                productsList[i].Add(ProductInformation[i].FindElement(By.ClassName("ppc_verpakking_titel")));
                productsList[i].Add(ProductInformation[i].FindElement(By.ClassName("BekijkBtn")));
                productsList[i].Add(ProductImage[i].FindElement(By.ClassName("ppc_images_verpakking")));
            }

            //LOOP THROUGH ALL THE AVAILABLE SALES -- PER PRODUCT
            for (int i = 0; i < StoreInformationGlobal.Count; i++)
            {
                //STORE ID CHANGES PER PRODUCT -- (verrander, verrander1, verrander2, etc...)
                string storeID = getStoreID(i);

                //FIND THE RIGHT DIV TO LOOK FOR INFOMATION 
                IWebElement winkelDiv = StoreInformationGlobal[i].FindElement(By.Id(storeID));

                //GET ALL THE STORES WITH A SALE BASED ON WHICH PRODUCT IT IS LOOKING AT
                ReadOnlyCollection<IWebElement> winkelsInformation = winkelDiv.FindElements(By.ClassName("bekijkWinkelsDiv"));

                //ADD AN EMPTY DICTIONARY TO SALESLIST -- WE PUT THE SALE IN THIS DICTIONARY
                //FOR EVERY SALE, A DICTIONARY IS CREATED
                salesList.Add(new Dictionary<IWebElement, IWebElement>());

                for (int j = 0; j < winkelsInformation.Count; j++)
                {
                    //IF THERE IS A SALE AVAILABLE -- ADD IT
                    try
                    {
                        salesList[i].Add(winkelsInformation[j].FindElement(By.ClassName("van_prijs")), winkelsInformation[j].FindElement(By.ClassName("aanbiedingPrijsPPC")));
                    }
                    //ELSE DON'T DO ANYTHING
                    catch (Exception e) { }
                }
            }


            //AT THIS POINT WE HAVE AL THE INFORMATION WE NEED, SO WE CAN ADD IT TO THE FINAL LIST!
            for (int i = 0; i < productsList.Count; i++)
            {
                //ADD LIST TO FINAL LIST
                //THIS SUBLIST WILL CONTAIN ALL THE RELEVANT INFORMATION PER PRODUCT;
                BierInformatie.Add(new List<object>());

                //GET THE NAME AND LOWEST PRICE AND ADD TO SUBLIST
                string name = productsList[i][PRODUCT_NAME].Text;
                string lowestPrice = productsList[i][PRODUCT_LOWEST_PRICE].Text;
                BierInformatie[i].Add(name);
                BierInformatie[i].Add(lowestPrice);

                //CREATE A LIST -- FOR POSSIBLE MULTIPLE SALES PER PRODUCT
                List<Dictionary<string, string>> aanbiedingen = new();
                for (int j = 0; j < salesList[i].Count; j++)
                {
                    //CREATE DICTIONARY FOR EVERY SALE
                    Dictionary<string, string> aanbieding = new();

                    //GET THE OLD AND NEW PRICE AND ADD THEM TO THE DICTIONARY
                    string van = salesList[i].ElementAt(j).Key.GetAttribute("innerHTML");
                    string voor = salesList[i].ElementAt(j).Value.GetAttribute("innerHTML");
                    aanbieding.Add(van, voor);
                    aanbiedingen.Add(aanbieding);
                }
                BierInformatie[i].Add(aanbiedingen);

                //GET THE IMAGE
                //string image = productsList[i][PRODUCT_IMAGE].GetAttribute("src");
                //BierInformatie[i].Add(image);
            }

            //CLOSE DRIVER AND RETURN FINAL LIST WITH INFORMATION
            biernetScraper.CloseDriver();
            return BierInformatie;
        }
        //USED TO GET THE CHANING STORE ID
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