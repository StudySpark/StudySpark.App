using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StudySpark.Core;
using StudySpark.Core.FileManager;
using StudySpark.GUI.WPF.Core;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class FilesDownloadViewModel {
        private object currentDownloadList;
        public object CurrentDownloadList {
            get {
                return currentDownloadList;
            }
            set {
                currentDownloadList = value;
            }
        }

        private const int amountToShow = 5;
        WrapPanel downloadPanel = new();
        Grid downloadGrid;

        public FilesDownloadViewModel() {
            SearchFiles searchFiles = new();
            List<string> _lastDownloadedFiles = searchFiles.GetLastDownloadedFiles(System.IO.SearchOption.TopDirectoryOnly);

            for (int i = 0; i < amountToShow; i++) {
                //create a grid for every iteration
                downloadGrid = new();
                downloadGrid.RowDefinitions.Add(new RowDefinition());
                downloadGrid.RowDefinitions.Add(new RowDefinition());
                if (i < _lastDownloadedFiles.Count) {

                    // Use Path.GetFileName to get only the file name part
                    string fileName = Path.GetFileName(_lastDownloadedFiles[i]);
                    string truncatedFileName = TruncateFileName(fileName, 15);

                    Button b = ButtonNoHoverEffect();
                    b.Tag = _lastDownloadedFiles[i];

                    RoutedEventHandler ClickOpenHandler = SystemFileHandler.CreateClickRawOpenHandler(_lastDownloadedFiles[i]);
                    b.Click += ClickOpenHandler;

                    downloadGrid.Children.Add(b);

                    //Create textbox and add it to grid
                    TextBlock t = SubText();
                    t.Text = truncatedFileName;
                    t.ToolTip = fileName;
                    downloadGrid.Children.Add(t);

                    //set row defenitions for button and text
                    Grid.SetRow(b, 0);
                    Grid.SetRow(t, 1);

                    //set alignment for panel
                    downloadPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    downloadPanel.VerticalAlignment = VerticalAlignment.Top;

                    //5 solutions need to be displayed
                    Thickness margin = downloadPanel.Margin;
                    margin.Top = 80;
                    downloadPanel.Margin = margin;

                    //add grid to panel
                    downloadPanel.Children.Add(downloadGrid);
                } else {
                    // If there are fewer than 5 files, create blank buttons or handle it as needed
                    downloadPanel.Children.Add(new Button {
                        Width = 50,
                        Height = 50,
                        IsEnabled = false, // Disable the button if no file is associated
                    });

                    downloadPanel.Children.Add(new TextBox {
                        TextAlignment = TextAlignment.Center,
                        Width = 100,
                        Height = 20,
                        Text = "No file available",
                        IsEnabled = false, // Disable the textbox as well
                    });
                }
            }
            currentDownloadList = downloadPanel;
        }
        public Button ButtonNoHoverEffect() {
            Button button = new Button();
            Style customButtonStyle = (Style)System.Windows.Application.Current.TryFindResource("FileButtonTheme");

            button.Width = 60;
            button.Height = 60;
            button.BorderThickness = new Thickness(0, 0, 0, 0);
            button.Cursor = Cursors.Hand;
            button.Background = SetIcon();
            button.Style = customButtonStyle;
            return button;
        }

        public TextBlock SubText() {
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
        private string TruncateFileName(string fileName, int maxLength) {
            if (fileName.Length <= maxLength) {
                return fileName;
            } else {
                // If the file name is too long, truncate it and add "..." at the end
                return fileName.Substring(0, maxLength - 3) + "...";
            }
        }

        public ImageBrush SetIcon() {

            ImageBrush? brush = null;
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                brush = new ImageBrush {
                    ImageSource = new BitmapImage(new Uri("StudySpark.GUI.WPF/Images/FileIcon.png", UriKind.Relative))
                };
            } else {
                try {
                    Logger.Warning($"Loading '{new Uri("..\\..\\..\\Images\\FileIcon.png", UriKind.Relative).AbsoluteUri.ToString()}' in VS mode");
                    brush = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri("..\\..\\..\\Images\\FileIcon.png", UriKind.Relative))
                    };
                } catch {
                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "FileIcon.png");
                    Logger.Warning($"Loading '{new Uri(imagePath).AbsoluteUri.ToString()}' in System mode");
                    brush = new ImageBrush { ImageSource = new BitmapImage(new Uri(imagePath)) };
                }
            }

            return brush;
        }
    }
}
