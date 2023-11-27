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

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class FilesFolderViewModel : ObservableObject
    {
        private object _currentFolderList;
        
        public RelayCommand OpenFolderSelectCommand { get; private set; }
        
        WrapPanel folderPanel = new WrapPanel();
        
        public List<GenericFile> files = new List<GenericFile>();
        
        private List<GenericFile> previousFiles;
        
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

            UpdateOnChange();

            //set alignment for panel
            folderPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            folderPanel.VerticalAlignment = VerticalAlignment.Top;

            _currentFolderList = folderPanel;

        }

        private void UpdateOnChange()
        {
            previousFiles = files;
            files = repository.ReadData();
            folderPanel.Children.Clear();
            
            List<GenericFile> difference = files.Except(previousFiles).ToList();
            foreach (GenericFile file in difference)
            {
                Style customButtonStyle = (Style)System.Windows.Application.Current.TryFindResource("FileButtonTheme");

                Grid folderGrid = new Grid();
                folderGrid.RowDefinitions.Add(new RowDefinition());
                folderGrid.RowDefinitions.Add(new RowDefinition());

                //Create button and add it to grid
                Button b = ButtonNoHoverEffect();
                b.Tag = file.Path;
                b.Style = customButtonStyle;
                folderGrid.Children.Add(b);

                //Create textbox and add it to grid
                TextBlock t = SubText();
                t.Text = TruncateFileName(file.TargetName, 15);
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

        public Button ButtonNoHoverEffect()
        {
            Button button = new Button();

            button.Width = 60;
            button.Height = 60;
            button.BorderThickness = new Thickness(0, 0, 0, 0);
            button.Background = SetIcon();
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

        public ImageBrush SetIcon()
        {

            ImageBrush? brush = null;
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("StudySpark.GUI.WPF/Images/DirectoryIcon.png", UriKind.Relative))
                };
            }
            else
            {
                brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/DirectoryIcon.png"))
                };
            }

            return brush;
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