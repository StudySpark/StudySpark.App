﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Accessibility;
using Newtonsoft.Json.Linq;
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

        public AlleBierAanbiedingenViewModel()
        {
            //CREATE SCRAPER AND RETRIEVE INFORMATION
            BiernetScraper.ScraperOptions options = new BiernetScraper.ScraperOptions();
            BiernetScraper scraper = new BiernetScraper(options);
            var BierInfo = scraper.BierScrape();

            var displayInfo = new StackPanel();

            for (int i = 0; i< BierInfo.Count; i++)
            {
                var info = DisplayInformation(BierInfo, i);
                
                displayInfo.Children.Add(info);
            }

            AllePanel.Children.Add(displayInfo);
            AlleAanbiedingen = AllePanel;
        }

        private UIElement DisplayInformation(List<List<object>> bierInfo, int index)
        {
            var container = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
            };
            container.Orientation = System.Windows.Controls.Orientation.Horizontal;
            container.Height = ENTRY_HEIGHT;

            Grid imageGrid = new();
            imageGrid.Width = IMAGE_WIDTH;
            //IMAGE OF THE PRODUCT
            var logo = new Image()
            {
                Width = IMAGE_WIDTH,
                Height = IMAGE_HEIGHT,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
            };
            //logo.Source = GetProductImage(bierInfo, index);
            logo.Source = new BitmapImage(new Uri("..\\..\\..\\Images\\man.png", UriKind.Relative));
            imageGrid.Children.Add(logo);
            container.Children.Add(imageGrid);

            //INFORMATION OF THE PRODUCT
            Grid infoGrid = new();
            infoGrid.Width = INFO_GRID_WIDTH;
            var information = DisplayInformationOfProduct(bierInfo, index);
            infoGrid.Children.Add(information);
            container.Children.Add(infoGrid);

            //SALES
            Grid salesGrid = new();
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(1, GridUnitType.Star);
            salesGrid.ColumnDefinitions.Add(c1);
            var prices = GetPrices(bierInfo, index);
            salesGrid.Children.Add(prices);
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

        private UIElement GetPrices(List<List<object>> bierInfo, int index)
        {
            int SALES = 2;
            var priceContainer = new WrapPanel();
            string? van = "";
            string? voor = "";
            for(int i = 0; i < bierInfo[index].Count; i++)
            {
                List<Dictionary<string, string>> sales = (List<Dictionary<string, string>>)bierInfo[index][SALES];
                for(int j=0; j < sales.Count; j++)
                {
                    van = sales[j].ElementAt(0).Key;
                    voor = sales[j].ElementAt(0).Value;
                    
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

        private BitmapImage GetProductImage(List<List<object>> bierInfo, int index)
        {
            int IMAGE_URL_INDEX = 2;
            string? url = bierInfo[index][IMAGE_URL_INDEX].ToString();

            Uri uri = new Uri(url);

            BitmapImage image = new BitmapImage(uri);

            return image;
        }
    }
}
