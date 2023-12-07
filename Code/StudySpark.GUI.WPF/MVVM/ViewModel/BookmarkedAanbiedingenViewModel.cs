using StudySpark.Core.Generic;
using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    class BookmarkedAanbiedingenViewModel : ObservableObject
    {
        public int IMAGE_WIDTH = 100;
        public int IMAGE_HEIGHT = 100;
        public int ENTRY_HEIGHT = 100;
        public int INFO_GRID_WIDTH = 175;
        public int SALES_GRID_WIDTH = 1;

        private BeerRepository beerRepository;

        private StackPanel bookmarkPanel = new StackPanel();

        private object alleBookmarks;
        public object AlleBookmarks
        {
            get
            {
                return alleBookmarks;
            }
            set
            {
                alleBookmarks = value;
                OnPropertyChanged();
            }
        }
        public BookmarkedAanbiedingenViewModel() 
        {
            beerRepository = new BeerRepository();
            List<GenericBeerProduct> bookmarks = beerRepository.getBookMarked();


            int z = 0;
            foreach (var bookmark in bookmarks)
            {
                var bookInfo = new StackPanel();

                List<GenericBeerSale> sales = beerRepository.getSales(bookmark.id);
                for (int i = 0; i < sales.Count; i++)
                {
                    if (sales.Count > 0)
                    {
                        var info = DisplayInformation(bookmark, sales, i, z);
                        bookInfo.Children.Add(info);
                    }
                }
                bookmarkPanel.Children.Add(new TextBlock()
                {
                    Text = bookmark.productname,
                    FontSize = 30,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Height = 40
                });
                bookmarkPanel.Children.Add(bookInfo);
            }
            alleBookmarks = bookmarkPanel;
        }

        private UIElement DisplayInformation(GenericBeerProduct product, List<GenericBeerSale> sales, int index, int zIndex)
        {
            //CREATE RETURN VALUE
            var container = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
            };
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
            var information = DisplayInformationOfProduct(product, sales, index);
            infoGrid.Children.Add(information);

            //SALES
            Grid salesGrid = new();
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(1, GridUnitType.Star);
            salesGrid.ColumnDefinitions.Add(c1);
            var prices = GetPrices(sales, index);
            salesGrid.Children.Add(prices);

            //ADD DIFFERENT GRIDS
            container.Children.Add(imageGrid);
            container.Children.Add(infoGrid);
            container.Children.Add(salesGrid);

            return container;
        }
        private UIElement DisplayInformationOfProduct(GenericBeerProduct product, List<GenericBeerSale> sales, int index)
        {
            var information = new StackPanel()
            {
                Height = ENTRY_HEIGHT,
            };
            var productNaam = new TextBlock()
            {
                Text = GetProductName(product, index),
            };
            productNaam.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            var laagstePrijs = new TextBlock()
            {
                Text = $"laagste prijs: {GetLowestPrice(product.lowestprice, index)}",
            };
            laagstePrijs.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            information.Children.Add(productNaam);
            information.Children.Add(laagstePrijs);

            return information;
        }
        private UIElement GetPrices(List<GenericBeerSale> sales, int index)
        {
            int SALES = 2;
            var priceContainer = new WrapPanel();
            string? van = "";
            string? voor = "";
            for (int i = 0; i < sales.Count; i++)
            {
                for (int j = 0; j < sales.Count; j++)
                {
                    van = sales[j].oldprice;
                    voor = sales[j].newprice;

                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri("..\\..\\..\\Images\\FileIcon.png", UriKind.Relative));
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
        private string GetProductName(GenericBeerProduct product, int index)
        {
            int NAME_INDEX = 0;
            string? name = product.productname;
            return name;
        }

        private string GetLowestPrice(string sale, int index)
        {
            int LOWEST_PRICE_INDEX = 1;
            string? lowestPrice = sale;

            int charLocationStart = lowestPrice.IndexOf("€");
            string lowestPriceTrimFirstHalf = lowestPrice.Substring(charLocationStart);
            int charLocationStop = lowestPriceTrimFirstHalf.IndexOf(":");
            string lowestPriceTrim = lowestPriceTrimFirstHalf.Substring(0, charLocationStop);

            return lowestPriceTrim;
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
