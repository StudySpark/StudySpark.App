using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StudySpark.Core.BierScraper;
using StudySpark.Core.Generic;
using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using StudySpark.GUI.WPF.MVVM.View;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    public class AlleBierAanbiedingenViewModel : ObservableObject
    {
        BeerRepository beerRepository = new BeerRepository();

        private event EventHandler ScraperHasFinished;
        private event EventHandler RetrieveFromDBHasFinished;

        private List<string> FilteredList = new();
        private BierFilterViewModel BierFilterVM;

        //GLOBAL VALUES
        private int IMAGE_WIDTH = 100;
        private int IMAGE_HEIGHT = 100;
        private int PRODUCT_INFORMATION_WIDTH = 300;
        private int PRODUCT_INFORMATION_HEIGHT = 100;
        private int INFO_GRID_WIDTH = 175;
        private int INFO_GRID_HEIGHT = 100;

        private object filterAanbiedingen;
        public object FilterAanbiedingen
        {
            get 
            { 
                return filterAanbiedingen;
            }
            set
            {
                filterAanbiedingen = value;
            }
        }

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
        
        //SUBLISTS -- PER BRAND -- ADD NEW LIST FOR EACH NEW BRAND
        private List<List<object>> BierInfoHertogJan;
        private List<List<object>> BierInfoAmstel;
        private List<List<object>> BierInfoHeineken;
        private List<List<object>> BierInfoGrolsch;

        private List<List<List<object>>> BierList = new();
        private List<GenericBeerProduct>? BierListFromDB = null;

        public static event EventHandler bookmarkAddedEvent;

        private StackPanel AllePanel = new StackPanel();
        public AlleBierAanbiedingenViewModel()
        {
            if (BierListFromDB == null)
            {
                BierListFromDB = RetrieveBeersalesFromDB();
            }

            BierFilterVM = new BierFilterViewModel();
            FilterAanbiedingen = BierFilterVM;

            Thread BierScrapeThread = new Thread(new ThreadStart(RetrieveBeerSales));
            BierScrapeThread.Start();


            //SUBSCRIBING TO EVENTS
            BierFilterView.ViewDataChangeEvent += SetFilteredList;   
            BierFilterView.ViewDataChangeEvent += DisplayBeerSales;
           
            BierAanbiedingenViewModel.BierAanbiedingenClickedEvent += DisplayBeerSales;

            RetrieveFromDBHasFinished += DisplayBeerSales;
            
            ScraperHasFinished += (object sender, EventArgs e) =>
            {
                BierListFromDB = RetrieveBeersalesFromDB();
            };
        }

        private void RetrieveBeerSales()
        {
            BiernetScraper.ScraperOptions options = new BiernetScraper.ScraperOptions();
            BiernetScraper scraper = new BiernetScraper(options);
            var salesInDB = RetrieveBeersalesFromDB();

            if (salesInDB.Count == 0)
            {
                
                List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierInfoHertogJan = scraper.BierScrape("https://www.biernet.nl/bier/merken/hertog-jan-pilsener");
                List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierInfoAmstel = scraper.BierScrape("https://www.biernet.nl/bier/merken/amstel-pilsener");
                List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierInfoHeineken = scraper.BierScrape("https://www.biernet.nl/bier/merken/heineken-pilsener");
                List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierInfoGrolsch = scraper.BierScrape("https://www.biernet.nl/bier/merken/grolsch-premium-pilsner");

            }

            ScraperHasFinished?.Invoke(this, new EventArgs());
        }
        private void DisplayBeerSales(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => {
                if (BierListFromDB.Count == 0)
                {
                    AllePanel.Children.Clear();
                    AllePanel.VerticalAlignment = VerticalAlignment.Top;
                    AllePanel.HorizontalAlignment = HorizontalAlignment.Center;
                    AllePanel.Children.Add(new TextBlock()
                    {
                        FontSize = 30,
                        Foreground = new SolidColorBrush(Colors.White),
                        Text = "Bieraanbiedingen worden opgehaald..."
                    });
                    AlleAanbiedingen = AllePanel;
                }
                else {
                    AllePanel.Children.Clear();
                    AllePanel.VerticalAlignment = VerticalAlignment.Center;
                    AllePanel.HorizontalAlignment = HorizontalAlignment.Center;
                    var displayInfo = new StackPanel();

                    for (int i = 0; i < BierListFromDB.Count; i++)
                    {
                        GenericBeerProduct beerSale = BierListFromDB[i];

                        //CHECK WHICH BRAND SHOULD BE DISPLAYED
                        string brandName = "";
                        switch (beerSale.brandID)
                        {
                            case 0:
                                brandName = "Hertog Jan";
                                break;
                            case 1:
                                brandName = "Amstel";
                                break;
                            case 2:
                                brandName = "Heineken";
                                break;
                            case 3:
                                brandName = "Grolsch";
                                break;
                        }

                        if (FilteredList.Contains(brandName))
                        {
                            StackPanel info = DisplayInformation(beerSale, i, brandName);
                            info.VerticalAlignment = VerticalAlignment.Center;
                            info.HorizontalAlignment = HorizontalAlignment.Left;
                            displayInfo.Children.Add(info);

                            if (i == 0)
                            {
                                AllePanel.Children.Add(new TextBlock()
                                {
                                    Text = brandName,
                                    FontSize = 30,
                                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                                    Height = 40
                                });
                            }
                        }
                       
                    }
                    AllePanel.Children.Add(displayInfo);
                }
                AlleAanbiedingen = AllePanel;
            });
        }
        private StackPanel DisplayInformation(GenericBeerProduct bierInfo, int index, string brandName)
        {
            //CREATE RETURN VALUE
            var containerDivider = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical,
                Height = PRODUCT_INFORMATION_HEIGHT + 15,
                //Width = double.NaN,
            };
            var container = new DockPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = PRODUCT_INFORMATION_HEIGHT,
                Width = 850,
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
                Source = GetProductImage(bierInfo.brandID)
            };
            Border b = new Border()
            {
                Height = IMAGE_HEIGHT,
                Width = IMAGE_WIDTH,
                //BorderBrush = new SolidColorBrush(Colors.DarkGray),
                //BorderThickness = new Thickness(1),
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
            var information = DisplayInformationOfProduct(bierInfo, index);
            infoGrid.Children.Add(information);

            //SALES
            Grid salesGrid = new();
            ColumnDefinition c1 = new()
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            salesGrid.ColumnDefinitions.Add(c1);
            var prices = GetPrices(bierInfo, index);
            salesGrid.Children.Add(prices);

            //BORDER
            Border border = new Border()
            {
                Height = PRODUCT_INFORMATION_HEIGHT,
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
            };
            // Bookmark button
            Image image = new Image();
            if (beerRepository.checkBookMark(bierInfo.productname, brandName)) {
                image.Source = new BitmapImage(new Uri("..\\..\\..\\Images\\bookmark_checked.png", UriKind.Relative));
            } else {
                image.Source = new BitmapImage(new Uri("..\\..\\..\\Images\\bookmark.png", UriKind.Relative));
            }

            Button bookmarkBtn = new Button()
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                BorderBrush = Brushes.Transparent,
                Width = 50,
                Height = 50,
                Content = image,
            };

            bookmarkBtn.Click += (sender, e) =>
            {
                BeerRepository beerRepository = new BeerRepository();
                if (!beerRepository.checkBookMark(bierInfo.productname, brandName))
                {
                    beerRepository.insertBookMark(brandName, bierInfo.productname, 1, bierInfo.lowestprice);
                    List<GenericBeerProduct> product = beerRepository.getLastBookMarked();

                    List<GenericBeerSale> sales = beerRepository.getSales(bierInfo.id);
                    for (int j = 0; j < sales.Count; j++)
                    {
                        beerRepository.insertSale(product[0].id, GetStoreImageString(sales[j], index, j), sales[j].oldprice, sales[j].newprice);
                    };

                    (bookmarkBtn.Content as Image).Source = new BitmapImage(new Uri("..\\..\\..\\Images\\bookmark_checked.png", UriKind.Relative));
                    // invoke event
                    bookmarkAddedEvent?.Invoke(this, new EventArgs());
                    MessageBox.Show("Bookmark added!");
                }
                else
                {
                    MessageBox.Show("Bookmark already added!");
                }
                
            };
            //ADD DIFFERENT GRIDS
            container.Children.Add(imageGrid);
            container.Children.Add(infoGrid);
            container.Children.Add(bookmarkBtn);
            container.Children.Add(salesGrid);

            border.Child = container;

            containerDivider.Children.Add(border);

            return containerDivider;
        }
        private Grid DisplayInformationOfProduct(GenericBeerProduct bierInfo, int index)
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
                Text = bierInfo.productname,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            };
            var laagstePrijs = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Text = $"laagste prijs: {bierInfo.lowestprice}",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            };

            information.Children.Add(productNaam);
            Grid.SetRow(productNaam, 0);
            information.Children.Add(laagstePrijs);
            Grid.SetRow(laagstePrijs, 1);
            
            return information;
        }
        private UIElement GetPrices(GenericBeerProduct bierInfo, int index)
        {
            int SALES = 2;
            var scrollViewer = new ScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                CanContentScroll = true,
                Height = 90,
            };

            var priceContainer = new WrapPanel() 
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            var sales = beerRepository.getSales(bierInfo.id);

            for(int j=0; j < sales.Count; j++)
            {

                Image img = new Image();
                try
                {
                    img.Source = GetStoreImage(sales[j], index, j);
                } catch (Exception e)
                {
                    img.Source = new BitmapImage(new Uri($"..\\..\\..\\Images\\man.png", UriKind.Relative));
                }
                img.Width = 40;
                
                Border b = new Border() 
                {
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(10),
                };
                b.Child = img; ;

                TextBlock t = new TextBlock();

                Run run1 = new Run($"{sales[j].oldprice}\n");
                run1.Foreground = new SolidColorBrush(Color.FromRgb(255, 160, 160));
                run1.TextDecorations = TextDecorations.Strikethrough;
                t.Inlines.Add(run1);

                Run run2 = new Run($"{sales[j].newprice}\n");
                run2.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                t.Inlines.Add(run2);


                priceContainer.Children.Add(b);
                priceContainer.Children.Add(t);

                scrollViewer.Content = priceContainer;
            }
            return scrollViewer;
        }
        private BitmapImage GetStoreImage(GenericBeerSale sale, int index, int jIndex)
        {
            string url = "https://www.biernet.nl/" + sale.store;

            BitmapImage image = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            return image;
        }
        private string GetStoreImageString(GenericBeerSale sale, int index, int jIndex)
        {
            string url = "https://www.biernet.nl/" + sale.store;
            return url;
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
                case 3:
                    name = "Grolsch";
                    break;
            }
            BitmapImage image = new BitmapImage(new Uri($"..\\..\\..\\Images\\{name}.png", UriKind.Relative));
            return image;
        }
        private void SetFilteredList(object sender, EventArgs e)
        {
            bool? HertogJanChecked = BierFilterViewModel.hertogIsChecked;
            bool? AmstelChecked = BierFilterViewModel.amstelIsChecked;
            bool? HeinekenChecked = BierFilterViewModel.heinekenIsChecked;
            bool? GrolschIsChecked = BierFilterViewModel.grolschIsChecked;

            if ((bool)HertogJanChecked)
            {
                if (!FilteredList.Contains("Hertog Jan"))
                {
                    FilteredList.Add("Hertog Jan");
                }
            }
            else
            {
                for(int i=0; i< FilteredList.Count; i++)
                {
                    if(FilteredList[i].Equals("Hertog Jan"))
                    {
                        FilteredList.RemoveAt(i);
                    }
                }
            }

            if ((bool)AmstelChecked)
            {
                if (!FilteredList.Contains("Amstel"))
                {
                    FilteredList.Add("Amstel");
                }
            } 
            else
            {
                for (int i = 0; i < FilteredList.Count; i++)
                {
                    if (FilteredList[i].Equals("Amstel"))
                    {
                        FilteredList.RemoveAt(i);
                    }
                }
            }

            if((bool)HeinekenChecked) 
            {
                if (!FilteredList.Contains("Heineken"))
                {
                    FilteredList.Add("Heineken");
                }
            }
            else
            {
                for (int i = 0; i < FilteredList.Count; i++)
                {
                    if (FilteredList[i].Equals("Heineken"))
                    {
                        FilteredList.RemoveAt(i);
                    }
                }
            }
            if ((bool)GrolschIsChecked)
            {
                if (!FilteredList.Contains("Grolsch"))
                {
                    FilteredList.Add("Grolsch");
                }
            }
            else
            {
                for (int i = 0; i < FilteredList.Count; i++)
                {
                    if (FilteredList[i].Equals("Grolsch"))
                    {
                        FilteredList.RemoveAt(i);
                    }
                }
            }
        }
        //private void AddBeersalesToDB(List<List<List<object>>> bierList)
        //{
        //    for (int z = 0; z < bierList.Count; z++)
        //    {
        //        string brandName = "";
        //        //CHECK WHICH BRAND SHOULD BE DISPLAYED
        //        switch (z)
        //        {
        //            case 0:
        //                brandName = "Hertog Jan";
        //                break;
        //            case 1:
        //                brandName = "Amstel";
        //                break;
        //            case 2:
        //                brandName = "Heineken";
        //                break;
        //            case 3:
        //                brandName = "Grolsch";
        //                break;
        //        }

        //        for (int i = 0; i < bierList[z].Count; i++)
        //        {
        //            var productname = bierList[z][i][0].ToString();
        //            var lowestPrice = bierList[z][i][1].ToString();
        //            var bookmarked = 0;
        //            var date = DateTime.Now;
                    
        //            var storeURLs = (List<List<string>>)bierList[z][i][3];
        //            var sales = (List<Dictionary<string, string>>)bierList[z][i][2];

        //            if (sales.Count > 0)
        //            {
        //                beerRepository.insertBeersale(brandName, productname, bookmarked, lowestPrice, date);
        //            }

        //            var lastInserted = beerRepository.getLastInserted();

        //            for (int j = 0; j < sales.Count; j++) {
        //                var storeURL = storeURLs[j][0];
        //                var van = sales[j].ElementAt(0).Key;
        //                var voor = sales[j].ElementAt(0).Value;
        //                var lastInsertedID = lastInserted[0].id;

        //                beerRepository.insertSale(lastInsertedID, storeURL, van, voor);
        //            }
        //        }
        //    }
        //}
        private List<GenericBeerProduct> RetrieveBeersalesFromDB()
        {
            var list = beerRepository.getBeerSales();
            return list;
        }
    }
}