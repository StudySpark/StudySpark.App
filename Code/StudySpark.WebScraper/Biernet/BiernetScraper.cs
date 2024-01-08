using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using StudySpark.WebScraper.BierScraper;
using StudySpark.WebScraper.Biernet;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using StudySpark.Core.Generic;

namespace StudySpark.WebScraper.Biernet
{
    public class BiernetScraper : WebScraper
    {
        public BiernetScraper(ScraperOptions scraperOptions) : base(scraperOptions)
        {
        }

        public void Load(string url, string brand)
        {
            this.brand = brand;
            scraperOptions.URL = url;
            SetupDriver(true);
            WaitForPageLoad();
        }
        private string brand;
        int PRODUCT_NAME = 0;
        int PRODUCT_LOWEST_PRICE = 1;

        public List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierScrape()
        {
            BierSalesScraper scraper = new BierSalesScraper(scraperOptions);

            List<List<object>> BierInformatie = new();

            List<List<IWebElement>> productsList = new();
            List<Dictionary<IWebElement, IWebElement>> salesList = new();
            List<List<IWebElement>> storeImages = new();

            IWebElement mobielDiv = GetElementById("verpakkingen_mobiel");
            ReadOnlyCollection<IWebElement> StoreInformationGlobal = mobielDiv.FindElements(By.TagName("li"));
            ReadOnlyCollection<IWebElement> ProductInformation = mobielDiv.FindElements(By.ClassName("ppc_text_verpakking"));

            List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BeerProductSales = new();
            for (int i = 0; i < ProductInformation.Count; i++)
            {
                string link = "";
                productsList.Add(new List<IWebElement>());
                try
                {
                    productsList[i].Add(ProductInformation[i].FindElement(By.ClassName("ppc_verpakking_titel")));
                    productsList[i].Add(ProductInformation[i].FindElement(By.ClassName("BekijkBtn")));
                    link = ProductInformation[i].FindElement(By.TagName("a")).GetAttribute("href");
                    GenericBeerProduct prod = getBeerProduct(i, productsList, brand);

                    scraper.Load(link);
                    List<GenericBeerSale> sales = scraper.BierSaleScrape();
                    scraper.driver.Close();
                    scraper.driver.Quit();

                    BeerProductSales.Add(new Dictionary<GenericBeerProduct, List<GenericBeerSale>>());
                    BeerProductSales[i].Add(prod, sales);
                }
                catch (NoSuchElementException ex) { }
                catch (Exception ex) { }

            }
            return BeerProductSales;

            //for (int i = 0; i < StoreInformationGlobal.Count; i++)
            //{
            //    string storeID = getStoreID(i);
            //    IWebElement winkelDiv = StoreInformationGlobal[i].FindElement(By.Id(storeID));
            //    ReadOnlyCollection<IWebElement> winkelsInformation = winkelDiv.FindElements(By.ClassName("bekijkWinkelsDiv"));

            //    salesList.Add(new Dictionary<IWebElement, IWebElement>());
            //    storeImages.Add(new List<IWebElement>());
            //    for (int j = 0; j < winkelsInformation.Count; j++)
            //    {
            //        try
            //        {
            //            salesList[i].Add(winkelsInformation[j].FindElement(By.ClassName("van_prijs")), winkelsInformation[j].FindElement(By.ClassName("aanbiedingPrijsPPC")));
            //            storeImages[i].Add(winkelsInformation[j].FindElement(By.ClassName("PakketFoto")));
            //        }
            //        catch (Exception) { }
            //    }
            //}
            //for (int i = 0; i < productsList.Count; i++)
            //{
            //    BierInformatie.Add(new List<object>());

            //    string name = productsList[i][PRODUCT_NAME].Text;
            //    string lowestPrice = productsList[i][PRODUCT_LOWEST_PRICE].Text;
            //    BierInformatie[i].Add(name);
            //    BierInformatie[i].Add(lowestPrice);

            //    List<Dictionary<string, string>> aanbiedingen = new();
            //    List<List<string>> images = new();
            //    for (int j = 0; j < salesList[i].Count; j++)
            //    {
            //        Dictionary<string, string> aanbieding = new();
            //        string van = salesList[i].ElementAt(j).Key.GetAttribute("innerHTML");
            //        string voor = salesList[i].ElementAt(j).Value.GetAttribute("innerHTML");
            //        aanbieding.Add(van, voor);
            //        aanbiedingen.Add(aanbieding);

            //        List<string> image = new();
            //        string? imageUrl = storeImages[i].ElementAt(j).GetAttribute("data-src");
            //        image.Add(imageUrl);
            //        images.Add(image);
            //    }
            //    BierInformatie[i].Add(aanbiedingen);
            //    BierInformatie[i].Add(images);
            //}
            //CloseDriver();
            //return BierInformatie;
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
