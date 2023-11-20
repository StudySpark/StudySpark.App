using StudySpark.Core.FileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace StudySpark.GUI.WPF.MVVM.View
{
    /// <summary>
    /// Interaction logic for FilesSolutionView.xaml
    /// </summary>
    public partial class FilesSolutionView : UserControl
    {
        private const int AmountToShow = 5;
        private List<string> _recentSLNFiles;
        public FilesSolutionView()
        {
            InitializeComponent();
            _recentSLNFiles = SearchFiles.GetFilesFromRecent(".sln.lnk", System.IO.SearchOption.TopDirectoryOnly);

            for (int i = 0; i < AmountToShow; i++)
            {
                Button b = new Button
                {
                    Width = 100,
                    Height = 100,
                };

                b.Background = SetIcon();

                SolutionPanel.Children.Add(b);
                SolutionPanel.Children.Add(new TextBox
                {
                    Text = _recentSLNFiles[i],
                }) ; 
            }
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
