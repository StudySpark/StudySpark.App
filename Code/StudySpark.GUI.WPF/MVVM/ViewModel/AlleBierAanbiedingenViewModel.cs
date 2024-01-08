using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LibGit2Sharp;
using StudySpark.WebScraper.Biernet;
using StudySpark.Core.Generic;
using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using StudySpark.GUI.WPF.MVVM.View;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    public class AlleBierAanbiedingenViewModel : ObservableObject
    {
        BeerRepository beerRepository = new BeerRepository();
        FilteredBeerList filters = new FilteredBeerList();

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
        private List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierInfoHertogJan;
        private List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierInfoAmstel;
        private List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierInfoHeineken;
        private List<Dictionary<GenericBeerProduct, List<GenericBeerSale>>> BierInfoGrolsch;

        private List<List<List<object>>> BierList = new();
        private List<GenericBeerProduct>? BierListFromDB = null;

        public static event EventHandler bookmarkAddedEvent;

        private StackPanel AllePanel = new StackPanel();
        public AlleBierAanbiedingenViewModel()
        {
            BierListFromDB = RetrieveBeersalesFromDB();

            BierFilterVM = new BierFilterViewModel();
            FilterAanbiedingen = BierFilterVM;

            var cts = new CancellationTokenSource();
            RecurringTask(() => StartScaper(), 60, cts.Token);  //interval in minutes

            //SUBSCRIBING TO EVENTS
            BierFilterView.ViewDataChangeEvent += SetFilteredList;
            BierFilterView.ViewDataChangeEvent += DisplayBeerSales;

            BierAanbiedingenViewModel.BierAanbiedingenClickedEvent += DisplayBeerSales;

            ScraperHasFinished += (object sender, EventArgs e) =>
            {
                BierListFromDB = RetrieveBeersalesFromDB();
            };
            ScraperHasFinished += DisplayBeerSales;
        }
        private void StartScaper()
        {
            Thread BierScrapeThread = new Thread(new ThreadStart(RetrieveBeerSales));
            BierScrapeThread.IsBackground = true;
            BierScrapeThread.Start();
        }
        private void RetrieveBeerSales()
        {
            WebScraper.ScraperOptions options = new();
            BiernetScraper scraper = new BiernetScraper(options);

            scraper.Load("https://www.biernet.nl/bier/merken/hertog-jan-pilsener", "Hertog Jan");
            BierInfoHertogJan = scraper.BierScrape();

            scraper.Load("https://www.biernet.nl/bier/merken/amstel-pilsener", "Amstel");
            BierInfoAmstel = scraper.BierScrape();

            scraper.Load("https://www.biernet.nl/bier/merken/heineken-pilsener", "Heineken");
            BierInfoHeineken = scraper.BierScrape();

            scraper.Load("https://www.biernet.nl/bier/merken/grolsch-premium-pilsner", "Grolsch");
            BierInfoGrolsch = scraper.BierScrape();

            BierList.Clear();
            //BierList.Add(BierInfoHertogJan);
            //BierList.Add(BierInfoAmstel);
            //BierList.Add(BierInfoHeineken);
            //BierList.Add(BierInfoGrolsch);

            AddBeersalesToDB();

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

                        if (true)//FilteredList.Contains(brandName))
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
                    beerRepository.updateBookMark(bierInfo.productname, bierInfo.brandID, 1);

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
            string url = sale.storeImage;

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
                case 3:
                    name = "Grolsch";
                    break;
            }
            BitmapImage image = new BitmapImage(new Uri($"..\\..\\..\\Images\\{name}.png", UriKind.Relative));
            return image;
        }
        private void SetFilteredList(object sender, EventArgs e)
        {
            bool? HertogJanChecked = BierFilterViewModel.hertogIsChecked ?? true;
            bool? AmstelChecked = BierFilterViewModel.amstelIsChecked ?? true;
            bool? HeinekenChecked = BierFilterViewModel.heinekenIsChecked ?? true;
            bool? GrolschIsChecked = BierFilterViewModel.grolschIsChecked ?? true;
            bool? KratIsChecked = BierFilterViewModel.kratIsChecked ?? true;
            bool? BlikIsChecked = BierFilterViewModel.blikIsChecked ?? true;
            bool? FlesIsChecked = BierFilterViewModel.flesIsChecked ?? true;
            bool? FustIsChecked = BierFilterViewModel.fustIsChecked ?? true;
            bool? TrayIsChecked = BierFilterViewModel.trayIsChecked ?? true;

            List<bool?> checkedFilters = new List<bool?>();
            checkedFilters.Add(HertogJanChecked);
            checkedFilters.Add(AmstelChecked);
            checkedFilters.Add(HeinekenChecked);
            checkedFilters.Add(GrolschIsChecked);
            checkedFilters.Add(KratIsChecked);
            checkedFilters.Add(BlikIsChecked);
            checkedFilters.Add(FlesIsChecked);
            checkedFilters.Add(FustIsChecked);
            checkedFilters.Add(TrayIsChecked);

            FilteredList = filters.SetFilteredList(checkedFilters);
        }

        private List<GenericBeerProduct> RetrieveBeersalesFromDB()
        {
            var list = beerRepository.getBeerSales();
            return list;
        }
        static void RecurringTask(Action action, int interval, CancellationToken cancellationToken)
        {
            if (action == null)
            {
                return;
            }

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    action();
                    await Task.Delay(TimeSpan.FromMinutes(interval), cancellationToken);
                }
            }, cancellationToken);
        }
        private void AddBeersalesToDB()
        {
            beerRepository.removeAll();

            beerRepository.insertBeersale(BierInfoHertogJan);
            beerRepository.insertBeersale(BierInfoAmstel);
            beerRepository.insertBeersale(BierInfoHeineken);
            beerRepository.insertBeersale(BierInfoGrolsch);
        }     
    }
}