﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        WrapPanel downloadPanel = new();
        Grid downloadGrid;

        public FilesDownloadViewModel()
        {
            for (int i = 0; i < AmountToShow; i++)
            {
                //create a grid for every iteration
                downloadGrid = new();
                downloadGrid.RowDefinitions.Add(new RowDefinition());
                downloadGrid.RowDefinitions.Add(new RowDefinition());
                if (i < _lastDownloadedFiles.Count)
                {

                    // Use Path.GetFileName to get only the file name part
                    string fileName = Path.GetFileName(_lastDownloadedFiles[i]);
                    string truncatedFileName = TruncateFileName(fileName, 15);

                    Button b = ButtonNoHoverEffect();
                    b.Tag = _lastDownloadedFiles[i];
                    downloadGrid.Children.Add(b);

                    //Create textbox and add it to grid
                    TextBlock t = SubText();
                    t.Text = truncatedFileName;
                    t.ToolTip = fileName;
                    downloadGrid.Children.Add(t);

                    //set row defenitions for button and text
                    Grid.SetRow(b, 0);
                    Grid.SetRow(t, 1);

                    //add grid to panel
                    downloadPanel.Children.Add(downloadGrid);
                }
                else
                {
                    downloadGrid = new();
                    downloadGrid.RowDefinitions.Add(new RowDefinition());
                    downloadGrid.RowDefinitions.Add(new RowDefinition());
                    // If there are fewer than 5 files, create blank buttons or handle it as needed

                    Button b = new Button()
                    {
                        Width = 60,
                        Height = 60,
                        IsEnabled = false,
                        BorderThickness = new Thickness(0, 0, 0, 0),
                    };
                    downloadGrid.Children.Add(b);

                    TextBlock t = SubText();
                    t.Text = "No file available";
                    downloadGrid.Children.Add(t);

                    Grid.SetRow(b, 0);
                    Grid.SetRow(t, 1);

                    downloadPanel.Children.Add(downloadGrid);
                }
                //set margin top -- in the middle of screen --> delete when more than
                //5 solutions need to be displayed
                Thickness margin = downloadPanel.Margin;
                margin.Top = 80;
                downloadPanel.Margin = margin;
                //------------------------------------

                //set alignment for panel
                downloadPanel.HorizontalAlignment = HorizontalAlignment.Center;
                downloadPanel.VerticalAlignment = VerticalAlignment.Top;
            }
            _currentDownloadList = downloadPanel;
        }
        public Button ButtonNoHoverEffect()
        {
            Button button = new Button();

            button.Width = 60;
            button.Height = 60;
            button.BorderThickness = new Thickness(0, 0, 0, 0);
            button.Cursor = Cursors.Hand;
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

        // Truncate the file name to the specified length
        private string TruncateFileName(string fileName, int maxLength)
        {
            if (fileName.Length <= maxLength)
            {
                return fileName;
            }
            else
            {
                // If the file name is too long, truncate it and add "..." at the end
                return fileName.Substring(0, maxLength - 3) + "...";
            }
        }
    }
}
