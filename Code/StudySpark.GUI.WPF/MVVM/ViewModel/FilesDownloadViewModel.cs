using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StudySpark.Core.FileManager;
using StudySpark.GUI.WPF.Core;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class FilesDownloadViewModel
    {
        private object _currentDownloadList;
        public object CurrentDownloadList
        {
            get
            {
                return _currentDownloadList;
            }
            set
            {
                _currentDownloadList = value;
            }
        }

        private const int AmountToShow = 5;
        private List<string> _lastDownloadedFiles = SearchFiles.GetLastDownloadedFiles(System.IO.SearchOption.TopDirectoryOnly);
        StackPanel downloadPanel = new();

        public FilesDownloadViewModel()
        {
            for (int i = 0; i < AmountToShow; i++)
            {
                if (i < _lastDownloadedFiles.Count)
                {
                    Button b = new Button
                    {
                        Width = 50,
                        Height = 50,
                    };
                    b.Tag = _lastDownloadedFiles[i];

                    downloadPanel.Children.Add(b);

                    // Use Path.GetFileName to get only the file name part
                    string fileName = Path.GetFileName(_lastDownloadedFiles[i]);

                    downloadPanel.Children.Add(new TextBox
                    {
                        TextAlignment = TextAlignment.Center,
                        Width = 200,
                        Height = 20,
                        Text = fileName,
                        IsEnabled = false
                    });
                }
                else
                {
                    // If there are fewer than 5 files, create blank buttons or handle it as needed
                    downloadPanel.Children.Add(new Button
                    {
                        Width = 50,
                        Height = 50,
                        IsEnabled = false, // Disable the button if no file is associated
                    });

                    downloadPanel.Children.Add(new TextBox
                    {
                        TextAlignment = TextAlignment.Center,
                        Width = 200,
                        Height = 20,
                        Text = "No file available",
                        IsEnabled = false, // Disable the textbox as well
                    });
                }
            }
            _currentDownloadList = downloadPanel;
        }
    }
}
