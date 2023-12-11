using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StudySpark.Core.BierScraper;
using StudySpark.GUI.WPF.Core;
using StudySpark.GUI.WPF.MVVM.View;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    
    class AlleBierAanbiedingenViewModel : ObservableObject
    {
        private event EventHandler ScraperHasFinished;

        private List<string> FilteredList = new();
        private BierFilterViewModel BierFilterVM;

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

        private StackPanel AllePanel = new StackPanel();
        public AlleBierAanbiedingenViewModel()
        {
            BierFilterVM = new BierFilterViewModel();
            FilterAanbiedingen = BierFilterVM;

            Thread BierScrapeThread = new Thread(new ThreadStart(RetrieveBeerSales));
            BierScrapeThread.Start();

            BierFilterView.ViewDataChangeEvent += SetFilteredList;   
            BierFilterView.ViewDataChangeEvent += DisplayBeerSales;
           
            BierAanbiedingenViewModel.BierAanbiedingenClickedEvent += DisplayBeerSales;
            ScraperHasFinished += DisplayBeerSales;
        }

        private void RetrieveBeerSales()
        {
            BiernetScraper.ScraperOptions options = new BiernetScraper.ScraperOptions();
            BiernetScraper scraper = new BiernetScraper(options);

            if (BierList.Count == 0)
            {
                BierInfoHertogJan = scraper.BierScrape("https://www.biernet.nl/bier/merken/hertog-jan-pilsener");
                BierInfoAmstel = scraper.BierScrape("https://www.biernet.nl/bier/merken/amstel-pilsener");
                BierInfoHeineken = scraper.BierScrape("https://www.biernet.nl/bier/merken/heineken-pilsener");
                BierInfoGrolsch = scraper.BierScrape("https://www.biernet.nl/bier/merken/grolsch-premium-pilsner");
            }

            BierList.Add(BierInfoHertogJan);
            BierList.Add(BierInfoAmstel);
            BierList.Add(BierInfoHeineken);
            BierList.Add(BierInfoGrolsch);

            ScraperHasFinished?.Invoke(this, new EventArgs());
        }
        private void DisplayBeerSales(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => {
                if (BierList.Count == 0)
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
                    AllePanel.HorizontalAlignment = HorizontalAlignment.Left;
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
                            case 3:
                                name = "Grolsch";
                                break;
                        }
                        var displayInfo = new StackPanel();

                        for (int i = 0; i < BierList[z].Count; i++)
                        {
                            var sales = (List<Dictionary<string, string>>)BierList[z][i][2];
                            if (sales.Count > 0)
                            {
                                if (FilteredList.Contains(name))
                                {
                                    StackPanel info = DisplayInformation(BierList[z], i, z);
                                    info.VerticalAlignment = VerticalAlignment.Center;
                                    info.HorizontalAlignment = HorizontalAlignment.Left;
                                    displayInfo.Children.Add(info);

                                    if (i == 0)
                                    {
                                        AllePanel.Children.Add(new TextBlock()
                                        {
                                            Text = name,
                                            FontSize = 30,
                                            Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                                            Height = 40
                                        });
                                    }
                                }
                            }
                        }
                        AllePanel.Children.Add(displayInfo);
                    }
                    AlleAanbiedingen = AllePanel;
                }
            });
        }
        private StackPanel DisplayInformation(List<List<object>> bierInfo, int index, int zIndex)
        {
            //CREATE RETURN VALUE
            var containerDivider = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical,
                Height = PRODUCT_INFORMATION_HEIGHT + 15,
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
                Source = GetProductImage(zIndex)
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
            var information = DisplayInformationOfProduct(bierInfo, index);
            infoGrid.Children.Add(information);

            //SALES
            Grid salesGrid = new();
            ColumnDefinition c1 = new()
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            salesGrid.ColumnDefinitions.Add(c1);
            var prices = GetPrices(bierInfo, index, zIndex);
            salesGrid.Children.Add(prices);

            //BORDER
            Border border = new Border()
            {
                Height = PRODUCT_INFORMATION_HEIGHT,
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
            };

            //ADD DIFFERENT GRIDS
            container.Children.Add(imageGrid);
            container.Children.Add(infoGrid);
            container.Children.Add(salesGrid);

            border.Child = container;

            containerDivider.Children.Add(border);

            return containerDivider;
        }
        private Grid DisplayInformationOfProduct(List<List<object>> bierInfo, int index)
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
                Text = GetProductName(bierInfo, index),
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            };

            var laagstePrijs = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Text = $"laagste prijs: {GetLowestPrice(bierInfo, index)}",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            };


            information.Children.Add(productNaam);
            Grid.SetRow(productNaam, 0);
            information.Children.Add(laagstePrijs);
            Grid.SetRow(laagstePrijs, 1);
            

            return information;
        }
        private UIElement GetPrices(List<List<object>> bierInfo, int index, int zIndex)
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
            string? van = "";
            string? voor = "";
            List<List<string>> images = (List<List<string>>) bierInfo[index][3];

            var sales = (List<Dictionary<string, string>>)bierInfo[index][SALES];
            for(int j=0; j < sales.Count; j++)
            {
                van = sales[j].ElementAt(0).Key;
                voor = sales[j].ElementAt(0).Value;

                Image img = new Image();
                try
                {
                    img.Source = GetStoreImage(bierInfo, index, j);
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
                   
                TextBlock t = new TextBlock()
                {
                    Text = $"Van: {van}\nVoor: {voor}\n",
                };
                t.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));


                priceContainer.Children.Add(b);
                priceContainer.Children.Add(t);

                scrollViewer.Content = priceContainer;
            }
            return scrollViewer;
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
        private BitmapImage GetStoreImage(List<List<object>> bierInfo, int index, int jIndex)
        {
            int IMAGES = 3;
            var images = (List<List<string>>)bierInfo[index][IMAGES];

            string temp = images[jIndex][0];
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
    }
}