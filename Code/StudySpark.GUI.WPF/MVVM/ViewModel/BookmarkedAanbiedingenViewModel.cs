using StudySpark.Core.Generic;
using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        public int PRODUCT_INFORMATION_WIDTH = 300;
        public int PRODUCT_INFORMATION_HEIGHT = 100;

        public int INFO_GRID_WIDTH = 175;
        public int INFO_GRID_HEIGHT = 100;

        public int SALES_GRID_WIDTH = 1;

        public int MARGIN = 1;

        private BeerRepository beerRepository;

        private StackPanel bookmarkPanel = new StackPanel();
        private event EventHandler bookmarkRemoved;
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
            AlleBierAanbiedingenViewModel.bookmarkAddedEvent += createBookMarkPanel;
            bookmarkRemoved += createBookMarkPanel;

            createBookMarkPanel(this, new EventArgs());
        }
        public void createBookMarkPanel(object? sender, EventArgs e)
        {
            bookmarkPanel.Children.Clear();
            List<GenericBeerProduct> bookmarks = beerRepository.getBookMarked();
            int z = 0;
            foreach (GenericBeerProduct bookmark in bookmarks)
            {
                var bookInfo = new StackPanel();

                List<GenericBeerSale> sales = beerRepository.getSales(bookmark.id);

                if (sales.Count > 0)
                {

                    var info = DisplayInformation(bookmark, sales, z);
                    bookInfo.Children.Add(info);
                }


                bookmarkPanel.Children.Add(bookInfo);
                z++;
            }
            alleBookmarks = bookmarkPanel;
        }
        private UIElement DisplayInformation(GenericBeerProduct product, List<GenericBeerSale> sales, int zIndex)
        {
            //CREATE RETURN VALUE
            var containerDivider = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical,
                Height = PRODUCT_INFORMATION_HEIGHT + 10,
            };

            var container = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal,
                Height = PRODUCT_INFORMATION_HEIGHT,
                Width = 650,
            };

            //IMAGE OF THE PRODUCT
            Grid imageGrid = new()
            {
                Width = IMAGE_WIDTH
            };
            var logo = new Image
            {
                Width = IMAGE_WIDTH,
                Height = IMAGE_HEIGHT,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Source = GetProductImage(product.brandID)
            };
            Border b = new Border()
            {
                Height = IMAGE_HEIGHT,
                Width = IMAGE_WIDTH,
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
            };
            b.Child = logo;
            imageGrid.Children.Add(b);


            //INFORMATION OF THE PRODUCT
            Grid infoGrid = new()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Width = INFO_GRID_WIDTH,
                Height = INFO_GRID_HEIGHT
            };
            var information = DisplayInformationOfProduct(product, zIndex);
            infoGrid.Children.Add(information);

            //SALES
            Grid salesGrid = new();
            ColumnDefinition c1 = new()
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            salesGrid.ColumnDefinitions.Add(c1);
            var prices = GetPrices(sales, zIndex);
            salesGrid.Children.Add(prices);

            Border border = new Border()
            {
                Height = PRODUCT_INFORMATION_HEIGHT,
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
            };
            // remove button
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("..\\..\\..\\Images\\Trash_Bin.png", UriKind.Relative));

            // Remove button
            Button removeBtn = new Button()
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                BorderBrush = Brushes.Transparent,
                Width = 50,
                Height = 50,
                Content = image,
            };

            removeBtn.Click += (sender, e) =>
            {
                beerRepository.removeBookMark(product.id);
                bookmarkRemoved?.Invoke(this, new EventArgs());
            };
            //ADD DIFFERENT GRIDS
            container.Children.Add(imageGrid);
            container.Children.Add(infoGrid);
            container.Children.Add(removeBtn);
            container.Children.Add(salesGrid);

            border.Child = container;

            containerDivider.Children.Add(border);

            return containerDivider;
        }
        private UIElement DisplayInformationOfProduct(GenericBeerProduct product, int index)
        {
            var information = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Height = PRODUCT_INFORMATION_HEIGHT,
                Width = PRODUCT_INFORMATION_WIDTH,
            };
            RowDefinition r1 = new()
            {
                Height = new GridLength(1, GridUnitType.Star)
            };
            RowDefinition r2 = new()
            {
                Height = new GridLength(1, GridUnitType.Star)
            };

            information.RowDefinitions.Add(r1);
            information.RowDefinitions.Add(r2);

            var productNaam = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = GetProductName(product, index),
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            };
            var laagstePrijs = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Text = $"laagste prijs: {GetLowestPrice(product.lowestprice, index)}",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            };
            information.Children.Add(productNaam);
            Grid.SetRow(productNaam, 0);
            information.Children.Add(laagstePrijs);
            Grid.SetRow(laagstePrijs, 1);

            return information;
        }
        private UIElement GetPrices(List<GenericBeerSale> sales, int index)
        {
            int SALES = 2;
            var priceContainer = new WrapPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            string? van = "";
            string? voor = "";

            for (int j = 0; j < sales.Count; j++)
            {
                van = sales[j].oldprice;
                voor = sales[j].newprice;

                Image img = new Image();
                img.Source = GetStoreImage(sales[j].store);
                img.Width = 40;

                Border b = new Border()
                {
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(10),
                };
                b.Child = img; ;

                TextBlock t = new TextBlock()
                {
                    Text = $"Van: {van}\nVoor: {voor}\n",
                };
                t.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                priceContainer.Children.Add(b);
                priceContainer.Children.Add(t);
            }
            

            return priceContainer;
        }
        private BitmapImage GetStoreImage(string url)
        {

            BitmapImage image = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            return image;
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
