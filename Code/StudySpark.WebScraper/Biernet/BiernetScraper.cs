using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace StudySpark.WebScraper.Biernet
{
    public class BiernetScraper : WebScraper
    {
        public BiernetScraper(ScraperOptions scraperOptions) : base(scraperOptions)
        {
        }

        public void Load(string url)
        {
            scraperOptions.URL = url;
            SetupDriver(true);
            WaitForPageLoad();
        }

        int PRODUCT_NAME = 0;
        int PRODUCT_LOWEST_PRICE = 1;

        public List<List<object>> BierScrape()
        {
            List<List<object>> BierInformatie = new();

            List<List<IWebElement>> productsList = new();
            List<Dictionary<IWebElement, IWebElement>> salesList = new();
            List<List<IWebElement>> storeImages = new();

            IWebElement mobielDiv = GetElementById("verpakkingen_mobiel");
            ReadOnlyCollection<IWebElement> StoreInformationGlobal = mobielDiv.FindElements(By.TagName("li"));
            ReadOnlyCollection<IWebElement> ProductInformation = mobielDiv.FindElements(By.ClassName("ppc_text_verpakking"));

            for (int i = 0; i < ProductInformation.Count; i++)
            {
                productsList.Add(new List<IWebElement>());
                productsList[i].Add(ProductInformation[i].FindElement(By.ClassName("ppc_verpakking_titel")));
                productsList[i].Add(ProductInformation[i].FindElement(By.ClassName("BekijkBtn")));
            }

            for (int i = 0; i < StoreInformationGlobal.Count; i++)
            {
                string storeID = getStoreID(i);
                IWebElement winkelDiv = StoreInformationGlobal[i].FindElement(By.Id(storeID));
                ReadOnlyCollection<IWebElement> winkelsInformation = winkelDiv.FindElements(By.ClassName("bekijkWinkelsDiv"));

                salesList.Add(new Dictionary<IWebElement, IWebElement>());
                storeImages.Add(new List<IWebElement>());
                for (int j = 0; j < winkelsInformation.Count; j++)
                {
                    try
                    {
                        salesList[i].Add(winkelsInformation[j].FindElement(By.ClassName("van_prijs")), winkelsInformation[j].FindElement(By.ClassName("aanbiedingPrijsPPC")));
                    }
                    catch (Exception e) { }
                    try
                    {
                        storeImages[i].Add(winkelsInformation[j].FindElement(By.ClassName("PakketFoto")));
                    }
                    catch (Exception e) { }
                }
            }
            for (int i = 0; i < productsList.Count; i++)
            {
                BierInformatie.Add(new List<object>());

                string name = productsList[i][PRODUCT_NAME].Text;
                string lowestPrice = productsList[i][PRODUCT_LOWEST_PRICE].Text;
                BierInformatie[i].Add(name);
                BierInformatie[i].Add(lowestPrice);

                List<Dictionary<string, string>> aanbiedingen = new();
                for (int j = 0; j < salesList[i].Count; j++)
                {
                    Dictionary<string, string> aanbieding = new();

                    string van = salesList[i].ElementAt(j).Key.GetAttribute("innerHTML");
                    string voor = salesList[i].ElementAt(j).Value.GetAttribute("innerHTML");
                    aanbieding.Add(van, voor);
                    aanbiedingen.Add(aanbieding);
                }
                BierInformatie[i].Add(aanbiedingen);

                List<List<string>> images = new();
                for (int j = 0; j < storeImages[i].Count; j++)
                {
                    List<string> image = new();
                    string? imageUrl = storeImages[i].ElementAt(j).GetAttribute("data-src");
                    image.Add(imageUrl);
                    images.Add(image);
                }
                BierInformatie[i].Add(images);
            }
            CloseDriver();
            return BierInformatie;
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
