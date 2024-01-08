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
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.Drawing.Drawing2D;

namespace StudySpark.WebScraper.BierScraper
{
    internal class BierSalesScraper : WebScraper
    {
        public BierSalesScraper(ScraperOptions scraperOptions) : base(scraperOptions) { }

        public void Load(string url)
        {
            scraperOptions.URL = url;
            SetupDriver(true);
            WaitForPageLoad();
        }
        public List<GenericBeerSale> BierSaleScrape()
        {
            List<List<object>> BierInformatie = new();
            List<GenericBeerSale> salesList = new List<GenericBeerSale>();

            int PRODUCT_NAME = 0;
            int PRODUCT_LOWEST_PRICE = 1;

            IWebElement ajaxdiv = GetElementById("inhoud_ajaxdiv");

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

                    
                }
                try
                {
                    IWebElement imageDiv = StoreInformationGlobal[i].FindElement(By.ClassName("logo_image"));
                    store = imageDiv.FindElement(By.TagName("img")).GetAttribute("alt");
                    storeimage = "https://www.biernet.nl" + imageDiv.FindElement(By.TagName("img")).GetAttribute("data-src");

                    IWebElement footer = StoreInformationGlobal[i].FindElement(By.ClassName("footer-item"));
                    string raw_expiration_date = footer.FindElement(By.ClassName("laatste_regel")).Text.Replace("t/m", "");
                    expiration_date = convertToDate(raw_expiration_date);
                } catch (NoSuchElementException e) { }

              
                salesList.Add(new GenericBeerSale(store, storeimage, oldprice, newprice, expiration_date));
            }

            return salesList;
        }
        public string convertToDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return "";
            }

            string dateString;
            
            var translations = new Dictionary<string, string>
            {
                {"maandag", "Monday"},
                {"dinsdag", "Tuesday"},
                {"woensdag", "Wednesday"},
                {"donderdag", "Thursday"},
                {"vrijdag", "Friday"},
                {"zaterdag", "Saturday"},
                {"zondag", "Sunday"},
                {"januari", "January"},
                {"februari", "February"},
                {"maart", "March"},
                {"april", "April"},
                {"mei", "May"},
                {"juni", "June"},
                {"juli", "July"},
                {"augustus", "August"},
                {"september", "September"},
                {"oktober", "October"},
                {"november", "November"},
                {"december", "December"}
            };
            
            string[] words = date.Split(' ');

            
            string day = translations[words[1]];
            string dayNum = words[2];
            string month = translations[words[3]];
            dateString = $"{day}, {month} {dayNum} {DateTime.Now.Year}";

            if (DateTime.TryParseExact(dateString, "dddd, MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result.ToString("yyyy-MM-dd");
            }
            return "";
        }
    }
}
