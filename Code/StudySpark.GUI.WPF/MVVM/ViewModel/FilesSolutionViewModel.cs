using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using StudySpark.Core.FileManager;
using StudySpark.GUI.WPF.Core;
using System.Windows;
using System.IO;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class FilesSolutionViewModel : ObservableObject
    {
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

        public Button ButtonNoHoverEffect()
        {
            Button button = new Button();

            button.Width = 60;
            button.Height = 60;
            button.BorderThickness = new Thickness(0, 0, 0, 0);
            button.Background = SetIcon();
            button.Cursor = Cursors.Hand;
            button.MouseDoubleClick += btn_Click;
            return button;
        }


        private const int AmountToShow = 5;
        private List<string> _recentSLNFiles = SearchFiles.GetFilesFromRecent(".sln.lnk", System.IO.SearchOption.TopDirectoryOnly);
        WrapPanel solutionPanel = new();
        Grid solutionGrid;

        public FilesSolutionViewModel()
        {

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
                    TextBlock t = new()
                    {
                        TextAlignment = TextAlignment.Center,
                        Width = 100,
                        Height = 20,
                        FontSize = 12,
                        Text = fileName,
                        Foreground = new SolidColorBrush(Colors.White),
                        Background = new SolidColorBrush(Colors.Transparent),
                        IsEnabled = true,
                        Cursor = Cursors.Hand
                    };
                    solutionGrid.Children.Add(t);

                    //set row defenitions for b and t
                    Grid.SetRow(b, 0);
                    Grid.SetRow(t, 1);

                    //set alignment for panel
                    solutionPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    solutionPanel.VerticalAlignment = VerticalAlignment.Top;
                    
                    //set margin top
                    Thickness margin = solutionPanel.Margin;
                    margin.Top = 80;
                    solutionPanel.Margin = margin; 

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
            var brush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\Images\\Icon_VS.png", UriKind.Relative))
            };
            return brush;
        }
        public ImageBrush SetIcon2()
        {
            var brush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("..\\..\\..\\Images\\DirectoryIcon.png", UriKind.Relative))
            };
            return brush;
        }
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
