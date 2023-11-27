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

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class FilesFolderViewModel : ObservableObject
    {
        private object currentFolderList;
        
        public object CurrentFolderList
        {
            get
            {
                return currentFolderList;
            }
            set
            {
                currentFolderList = value;
            }
        }

        WrapPanel folderPanel = new WrapPanel();
        public List<GenericFile> files = new List<GenericFile>(); 
        FileRepository repository;
        public FilesFolderViewModel()
        {
            repository = new FileRepository();
            List<GenericFile> files = repository.ReadData();
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));
            //files.Add(new GenericFile(1, "d", "c", "b", "a"));     
            foreach (GenericFile file in files)
            {
                Grid folderGrid = new Grid();
                folderGrid.RowDefinitions.Add(new RowDefinition());
                folderGrid.RowDefinitions.Add(new RowDefinition());
                //create button and add it to grid
                Button b = ButtonNoHoverEffect();
                b.Tag = file.Path;
                folderGrid.Children.Add(b);

                //Create textbox and add it to grid
                TextBlock t = SubText();
                t.Text = TruncateFileName(file.Path, 15);
                t.ToolTip = file.Path;
                folderGrid.Children.Add(t);

                //set row defenitions for button and text
                Grid.SetRow(b, 0);
                Grid.SetRow(t, 1);

                //
                Thickness margin = folderGrid.Margin;
                margin.Bottom = 75;
                folderGrid.Margin = margin;


                //set alignment for panel
                folderPanel.HorizontalAlignment = HorizontalAlignment.Center;
                folderPanel.VerticalAlignment = VerticalAlignment.Top;

                //add grid to panel
                folderPanel.Children.Add(folderGrid);

            }
            currentFolderList = folderPanel;
        }
        public Button ButtonNoHoverEffect()
        {
            Button button = new Button();

            button.Width = 60;
            button.Height = 60;
            button.BorderThickness = new Thickness(0, 0, 0, 0);
            button.Background = SetIcon();
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
    }
}