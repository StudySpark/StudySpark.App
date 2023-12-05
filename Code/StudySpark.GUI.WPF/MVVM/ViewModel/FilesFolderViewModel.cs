using StudySpark.GUI.WPF.Core;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System;
using StudySpark.Core.FileManager;
using StudySpark.Core.Repositories;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using System.Windows.Threading;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class FilesFolderViewModel : ObservableObject
    {
        private object _currentFolderList;
        
        public RelayCommand OpenFolderSelectCommand { get; private set; }

        public RelayCommand OpenFileSelectCommand { get; private set; }
        
        WrapPanel folderPanel = new WrapPanel();
        
        public List<GenericFile> files = new List<GenericFile>();

        FileRepository repository = new FileRepository();

        public object CurrentFolderList
        {
            get
            {
                return _currentFolderList;
            }
            set
            {
                _currentFolderList = value;
            }
        }

        public FilesFolderViewModel()
        {
            OpenFolderSelectCommand = new RelayCommand(o => SelectFolder());
            OpenFileSelectCommand = new RelayCommand(o => SelectFile());

            UpdateOnChange();

            //set alignment for panel
            folderPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            folderPanel.VerticalAlignment = VerticalAlignment.Top;

            _currentFolderList = folderPanel;

        }

        private void UpdateOnChange()
        {
            files = repository.ReadData();
            folderPanel.Children.Clear();

            foreach (GenericFile file in files)
            {
                Style customButtonStyle = (Style)System.Windows.Application.Current.TryFindResource("FileButtonTheme");

                Grid folderGrid = new Grid();
                folderGrid.RowDefinitions.Add(new RowDefinition());
                folderGrid.RowDefinitions.Add(new RowDefinition());

                //Create button and add it to grid
                Button b = ButtonNoHoverEffect(file.Image);
                b.Tag = file.Path;
                b.Style = customButtonStyle;

                RoutedEventHandler ClickHandler = (sender, args) =>
                {
                    if (args.OriginalSource is Button clickedButton && clickedButton.Tag is string folderPath && file.TargetName is string fileName)
                    {

                        string buttonFilePath = System.IO.Path.Combine(folderPath, fileName);

                        // Logic to run the file using the buttonFilePath
                        System.Windows.MessageBox.Show($"Running file: {buttonFilePath}");

                        try
                        {
                            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
                            {
                                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/c start \"\" \"{buttonFilePath}\"",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true
                                };

                                process.StartInfo = startInfo;
                                process.Start();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Error: {ex.Message}");
                        }
                    }
                };

                folderGrid.AddHandler(Button.MouseLeftButtonDownEvent, ClickHandler);

                b.Click += (sender, e) =>
                {
                    if (b.Tag is string filePath)
                    {
                        ClickHandler?.Invoke(this, e);
                    }
                };

                folderGrid.Children.Add(b);

                //Create textbox and add it to grid
                TextBlock t = SubText();
                if (file.TargetName.Length > 0) 
                { 
                t.Text = TruncateFileName(file.TargetName, 15);
                }
                else { t.Text = file.Path; }
                t.ToolTip = file.Path;
                folderGrid.Children.Add(t);

                //set row defenitions for button and text
                Grid.SetRow(b, 0);
                Grid.SetRow(t, 1);

                //
                Thickness margin = folderGrid.Margin;
                margin.Bottom = 75;
                folderGrid.Margin = margin;

                //add grid to panel
                folderPanel.Children.Add(folderGrid);
            }
        }

        public Button ButtonNoHoverEffect(string image)
        {
            Button button = new Button();

            button.Width = 60;
            button.Height = 60;
            button.BorderThickness = new Thickness(0, 0, 0, 0);
            button.Background = SetIcon(image);
            button.Cursor = System.Windows.Input.Cursors.Hand;
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
            textBlock.Cursor = System.Windows.Input.Cursors.Hand;
            return textBlock;
        }

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

        public ImageBrush SetIcon(string image)
        {

            ImageBrush? brush = null;
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri($"StudySpark.GUI.WPF/Images/{image}", UriKind.Relative))
                };
            }
            else
            {
                brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Images/{image}"))
                };
            }

            return brush;
        }

        private void SelectFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string path = ofd.FileName;
                int pos = path.LastIndexOf('.');
                string ext = path.Substring(pos + 1);

                if (!string.IsNullOrEmpty(path))
                {
                    bool result = repository.InsertData(path, ext);
                    if (!result)
                    {
                        System.Windows.MessageBox.Show("Er is iets fout gegaan!");
                        return;
                    }
                    UpdateOnChange();
                }
                System.Windows.MessageBox.Show("Bestand succesvol toegevoegd!");
            }
            else
            {
                System.Windows.MessageBox.Show("Er is iets fout gegaan!");
            }


        }

        private void SelectFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            
            DialogResult dr = fbd.ShowDialog();
            
            if (dr == DialogResult.OK) 
            {
                string path = fbd.SelectedPath;
                if (!string.IsNullOrEmpty(path))
                {
                    bool result = repository.InsertData(path, "folder", "DirectoryIcon.png");
                    if (!result)
                    {
                        System.Windows.MessageBox.Show("Er is iets fout gegaan!");
                        return;
                    }
                    UpdateOnChange();
                }
                System.Windows.MessageBox.Show("Map succesvol toegevoegd!");
            } else
            {
                System.Windows.MessageBox.Show("Er is iets fout gegaan!");
            }
        }

    }
}