using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using StudySpark.Core.FileManager;
using StudySpark.GUI.WPF.Core;
using System.Windows;
using System.IO;
using System.ComponentModel;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    internal class FilesSolutionViewModel : ObservableObject {
        private object _currentSLNList;
        public object CurrentSLNList {
            get {
                return _currentSLNList;
            }
            set {
                _currentSLNList = value;
            }
        }

        private const int AmountToShow = 5;
        private List<string> _recentSLNFiles = SearchFiles.GetFilesFromRecent(".sln.lnk", System.IO.SearchOption.TopDirectoryOnly);
        StackPanel solutionPanel = new();

        public FilesSolutionViewModel() {
            for (int i = 0; i < AmountToShow; i++) {
                if (i < _recentSLNFiles.Count) {
                    Button b = new Button {
                        Width = 50,
                        Height = 50,
                    };

                    b.Background = SetIcon();
                    b.Tag = _recentSLNFiles[i];

                    solutionPanel.Children.Add(b);

                    // Use Path.GetFileName to get only the file name part
                    string fileName = Path.GetFileName(_recentSLNFiles[i]);

                    solutionPanel.Children.Add(new TextBox {
                        TextAlignment = TextAlignment.Center,
                        Width = 150,
                        Height = 20,
                        Text = fileName,
                        IsEnabled = false
                    });
                } else {
                    // If there are fewer than 5 files, create blank buttons or handle it as needed
                    solutionPanel.Children.Add(new Button {
                        Width = 50,
                        Height = 50,
                        IsEnabled = false, // Disable the button if no file is associated
                    });

                    solutionPanel.Children.Add(new TextBox {
                        TextAlignment = TextAlignment.Center,
                        Width = 150,
                        Height = 20,
                        Text = "No file available",
                        IsEnabled = false, // Disable the textbox as well
                    });
                }
            }
            _currentSLNList = solutionPanel;
        }

        public ImageBrush SetIcon() {

            ImageBrush? brush = null;
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                brush = new ImageBrush {
                    ImageSource = new BitmapImage(new Uri("StudySpark.GUI.WPF/Images/Icon_VS.png", UriKind.Relative))
                };
            } else {
                brush = new ImageBrush {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Icon_VS.png"))
                };
            }

            return brush;
        }
    }
}
