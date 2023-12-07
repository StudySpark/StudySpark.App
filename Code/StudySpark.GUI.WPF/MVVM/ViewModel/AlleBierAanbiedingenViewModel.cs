using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using StudySpark.Core.BierScraper;
using StudySpark.GUI.WPF.Core;


namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    class AlleBierAanbiedingenViewModel : ObservableObject
    {
        public int IMAGE_WIDTH = 100;
        public int IMAGE_HEIGHT = 100;
        public int ENTRY_HEIGHT = 100;
        public int INFO_GRID_WIDTH = 175;
        public int SALES_GRID_WIDTH = 1;

        private object alleAanbiedingen;
        public object AlleAanbiedingen
        {
            get
            {
                return alleAanbiedingen;
            }
            set
            {
                alleAanbiedingen = value;
                OnPropertyChanged();
            }
        }
        private StackPanel AllePanel = new StackPanel();

        private enum BierEnum
        {
            HERTOG_JAN = 0,
            AMSTEL = 1
        }

        public AlleBierAanbiedingenViewModel()
        {
            BierAanbiedingenViewModel.BierAanbiedingenClickedEvent += DisplayBeerSales;
        }



        public void DisplayBeerSales(object? sender, EventArgs e) {
            //CREATE SCRAPER AND RETRIEVE INFORMATION
            List<List<List<object>>> BierList = new();
            Thread t = new Thread(() =>
            {
                BiernetScraper.ScraperOptions options = new BiernetScraper.ScraperOptions();
                BiernetScraper scraper = new BiernetScraper(options);

                var BierInfoHertogJan = scraper.BierScrape("https://www.biernet.nl/bier/merken/hertog-jan-pilsener");
                var BierInfoAmstel = scraper.BierScrape("https://www.biernet.nl/bier/merken/amstel-pilsener");
                var BierInfoHeineken = scraper.BierScrape("https://www.biernet.nl/bier/merken/heineken-pilsener");

                BierList.Add(BierInfoHertogJan);
                BierList.Add(BierInfoAmstel);
                BierList.Add(BierInfoHeineken);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            for (int z = 0; z < BierList.Count; z++)
            {
                string name = "";
                //CHECK WHICH BRAND SHOULD BE DISPLAYED
                switch (z)
                {
                    case 0:
                        name = "Hertog Jan";
                        break;
                    case 1:
                        name = "Amstel";
                        break;
                    case 2:
                        name = "Heineken";
                        break;
                }
                var displayInfo = new StackPanel();
                for (int i = 0; i < BierList[z].Count; i++)
                {
                    List<Dictionary<string, string>> sales = (List<Dictionary<string, string>>)BierList[z][i][2];
                    if (sales.Count > 0)
                    {
                        var info = DisplayInformation(BierList[z], i, z);
                        displayInfo.Children.Add(info);
                    }
                }
                AllePanel.Children.Add(new TextBlock()
                {
                    Text = name,
                    FontSize = 30,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Height = 40
                }); 
                AllePanel.Children.Add(displayInfo);
            }
            AlleAanbiedingen = AllePanel;
        }
        private UIElement DisplayInformation(List<List<object>> bierInfo, int index, int zIndex)
        {
            var container = new StackPanel();

            //CREATE RETURN VALUE
            container.VerticalAlignment = VerticalAlignment.Center;
            container.Orientation = System.Windows.Controls.Orientation.Horizontal;
            container.Height = ENTRY_HEIGHT;

            //IMAGE OF THE PRODUCT
            Grid imageGrid = new();
            imageGrid.Width = IMAGE_WIDTH;
            var logo = new Image()
            {
                Width = IMAGE_WIDTH,
                Height = IMAGE_HEIGHT,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
            };
            logo.Source = GetProductImage(zIndex);
            imageGrid.Children.Add(logo);


            //INFORMATION OF THE PRODUCT
            Grid infoGrid = new();
            infoGrid.Width = INFO_GRID_WIDTH;
            var information = DisplayInformationOfProduct(bierInfo, index);
            infoGrid.Children.Add(information);

            //SALES
            Grid salesGrid = new();
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(1, GridUnitType.Star);
            salesGrid.ColumnDefinitions.Add(c1);
            var prices = GetPrices(bierInfo, index, zIndex);
            salesGrid.Children.Add(prices);

            //ADD DIFFERENT GRIDS
            container.Children.Add(imageGrid);
            container.Children.Add(infoGrid);
            container.Children.Add(salesGrid);

            return container;
        }
        private UIElement DisplayInformationOfProduct(List<List<object>> bierInfo, int index)
        {
            var information = new StackPanel()
            {
                Height = ENTRY_HEIGHT,
            };
            var productNaam = new TextBlock()
            {
                Text = GetProductName(bierInfo, index),
            };
            productNaam.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            var laagstePrijs = new TextBlock()
            {
                Text = $"laagste prijs: {GetLowestPrice(bierInfo, index)}",
            };
            laagstePrijs.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            information.Children.Add(productNaam);
            information.Children.Add(laagstePrijs);

            return information;
        }
        private UIElement GetPrices(List<List<object>> bierInfo, int index, int zIndex)
        {
            int SALES = 2;
            int IMAGES = 3;
            var priceContainer = new WrapPanel();
            string? van = "";
            string? voor = "";
            for(int i = 0; i < bierInfo[index].Count; i++)
            {
                List<Dictionary<string, string>> sales = (List<Dictionary<string, string>>)bierInfo[index][SALES];
                List<List<string>> image = (List<List<string>>)bierInfo[index][IMAGES];
                for(int j=0; j < sales.Count; j++)
                {
                    van = sales[j].ElementAt(0).Key;
                    voor = sales[j].ElementAt(0).Value;

                    Image img = new Image();
                    try
                    {
                        img.Source = GetStoreImage(bierInfo, index, zIndex);
                    } catch (Exception e)
                    {
                        img.Source = new BitmapImage(new Uri($"..\\..\\..\\Images\\man.png", UriKind.Relative));
                    }
                    img.Width = 40;
                   
                    TextBlock t = new TextBlock()
                    {
                        Text = $"Van: {van}\nVoor: {voor}\n",
                    };
                    t.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                    priceContainer.Children.Add(img);
                    priceContainer.Children.Add(t);
                }
            }

            return priceContainer;
        }
        private string GetProductName(List<List<object>> bierInfo, int index)
        {
            int NAME_INDEX = 0;
            string? name = bierInfo[index][NAME_INDEX].ToString();
            return name;
        }

        private string GetLowestPrice(List<List<object>> bierInfo, int index)
        {
            int LOWEST_PRICE_INDEX = 1;
            string? lowestPrice = bierInfo[index][LOWEST_PRICE_INDEX].ToString();

            int charLocationStart = lowestPrice.IndexOf("€");
            string lowestPriceTrimFirstHalf = lowestPrice.Substring(charLocationStart);
            int charLocationStop = lowestPriceTrimFirstHalf.IndexOf(":");
            string lowestPriceTrim = lowestPriceTrimFirstHalf.Substring(0, charLocationStop);

            return lowestPriceTrim;
        }

        private BitmapImage GetStoreImage(List<List<object>> bierInfo, int index, int zIndex)
        {
            int IMAGE_INDEX = 3;
            List<List<string>> images = (List<List<string>>)bierInfo[zIndex][IMAGE_INDEX];

            string temp = images[index][0];
            string url = "https://www.biernet.nl/"+temp;

            BitmapImage image = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            return image;
        }

        private BitmapImage GetProductImage(int index)
        {
            string name = "";
            switch (index)
            {
                case 0:
                    name = "HertogJan";
                    break;
                case 1:
                    name = "Amstel";
                    break;
                case 2:
                    name = "Heineken";
                    break;
            }
            BitmapImage image = new BitmapImage(new Uri($"..\\..\\..\\Images\\{name}.png", UriKind.Relative));
            return image;
        }
    }
}
