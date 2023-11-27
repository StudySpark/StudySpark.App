using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using StudySpark.Core.FileManager;
using StudySpark.GUI.WPF.Core;
using System.Windows;
using System.IO;
using System.Windows.Input;
using System.ComponentModel;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class FilesSolutionViewModel : ObservableObject
    {
        SearchFiles searchFiles = new();
        private object _currentSLNList;
        public object CurrentSLNList {
            get
            {
                return _currentSLNList;
            }
            set
            {
                _currentSLNList = value;
            }
        }

        private const int AmountToShow = 5;
        
        WrapPanel solutionPanel = new();
        Grid solutionGrid;

        public FilesSolutionViewModel()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            string extension = ".sln.lnk";
            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            List<string> _recentSLNFiles = searchFiles.GetFilesFromDir(path, extension, searchOption);
            
            for (int i = 0; i < AmountToShow; i++)
            {
                //create a grid for every iteration
                solutionGrid = new();
                solutionGrid.RowDefinitions.Add(new RowDefinition());
                solutionGrid.RowDefinitions.Add(new RowDefinition());
                if (i < _recentSLNFiles.Count)
                {
                    // First use Path.GetFileName to get only the file name part
                    string fileName = Path.GetFileName(_recentSLNFiles[i]);
                    fileName = fileName.Substring(0, fileName.IndexOf(".lnk"));

                    //create button and add it to grid
                    Button b = ButtonNoHoverEffect();
                    b.Tag = _recentSLNFiles[i];
                    solutionGrid.Children.Add(b);

                    //Create textbox and add it to grid
                    TextBlock t = SubText();
                    t.Text = fileName;
                    solutionGrid.Children.Add(t);

                    //set row defenitions for button and text
                    Grid.SetRow(b, 0);
                    Grid.SetRow(t, 1);

                    //set alignment for panel
                    solutionPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    solutionPanel.VerticalAlignment = VerticalAlignment.Top;
                    
                    //set margin top -- in the middle of screen --> delete when more than
                    //5 solutions need to be displayed
                    Thickness margin = solutionPanel.Margin;
                    margin.Top = 80;
                    solutionPanel.Margin = margin;
                    //------------------------------------

                    //add grid to panel
                    solutionPanel.Children.Add(solutionGrid);
                }
                else
                {
                    // If there are fewer than 5 files, create blank buttons or handle it as needed
                    solutionGrid.Children.Add(new Button
                    {
                        Width = 50,
                        Height = 50,
                        IsEnabled = false, // Disable the button if no file is associated
                    });

                    solutionGrid.Children.Add(new TextBox
                    {
                        TextAlignment = TextAlignment.Center,
                        Width = 100,
                        Height = 20,
                        Text = "No file available",
                        IsEnabled = false, // Disable the textbox as well
                    });
                    solutionPanel.Children.Add(solutionGrid);
                }
            }
            _currentSLNList = solutionPanel;
        }

        public ImageBrush SetIcon()
        {
            ImageBrush? brush = null;
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("StudySpark.GUI.WPF/Images/Icon_VS.png", UriKind.Relative))
                };
            }
            else
            {
                brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("..\\..\\..\\Images\\Icon_VS.png", UriKind.Relative))
                };
            }
            return brush;
        }

        public Button ButtonNoHoverEffect()
        {
            Button button = new Button();
            Style customButtonStyle = (Style)System.Windows.Application.Current.TryFindResource("FileButtonTheme");

            button.Width = 60;
            button.Height = 60;
            button.BorderThickness = new Thickness(0, 0, 0, 0);
            button.Background = SetIcon();
            button.Cursor = Cursors.Hand; 
            button.MouseDoubleClick += btn_Click;
            button.Style = customButtonStyle;
            return button;
        }

        public TextBlock SubText()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.Width = 100;
            textBlock.Height = 20;
            textBlock.FontSize = 12;
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.Background = new SolidColorBrush(Colors.Transparent);
            textBlock.IsEnabled = true;
            textBlock.Cursor = Cursors.Hand;
            return textBlock;
        }


        //for testing purposes -- delete when merging
        public ImageBrush SetIcon2()
        {
            var brush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\Images\\DirectoryIcon.png", UriKind.Relative))
            };
            return brush;
        }
        //--------------------------------------------

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                button.Background = SetIcon2();
            }
        }
    }
}
