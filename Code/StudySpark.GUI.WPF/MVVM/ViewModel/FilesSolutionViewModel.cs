using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using StudySpark.Core.FileManager;
using StudySpark.GUI.WPF.Core;
using System.Windows;

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

        private const int AmountToShow = 5;
        private List<string> _recentSLNFiles = SearchFiles.GetFilesFromRecent(".sln.lnk", System.IO.SearchOption.TopDirectoryOnly);
        StackPanel solutionPanel = new();

        public FilesSolutionViewModel()
        {
            for (int i = 0; i < AmountToShow; i++)
            {
                Button b = new Button
                {
                    Width = 50,
                    Height = 50,
                };

                b.Background = SetIcon();

                solutionPanel.Children.Add(b);
                solutionPanel.Children.Add(new TextBox
                {
                    Text = _recentSLNFiles[i]
                }); 
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
    }
}
