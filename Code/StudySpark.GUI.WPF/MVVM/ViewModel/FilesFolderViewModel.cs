using StudySpark.GUI.WPF.Core;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
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
using System.IO;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    public class FilesFolderViewModel : ObservableObject
    {
        private object _currentFolderList;
        
        public RelayCommand OpenFolderSelectCommand { get; private set; }

        public RelayCommand OpenFileSelectCommand { get; private set; }
        
        WrapPanel folderPanel = new WrapPanel();
        
        public List<GenericFile> files = new List<GenericFile>();

        public Hashtable fileIDs = new Hashtable();

        private List<Button> DeletableButtons = new List<Button>();

        private ToggleButton DeleteButton;

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
            files = DBConnector.Database.ReadFileData();
            folderPanel.Children.Clear();
            fileIDs.Clear();
            int fileID = 0;

            foreach (GenericFile file in files)
            {
                Style customButtonStyle = (Style)System.Windows.Application.Current.TryFindResource("FileButtonTheme");

                Grid folderGrid = new Grid();
                folderGrid.RowDefinitions.Add(new RowDefinition());
                folderGrid.RowDefinitions.Add(new RowDefinition());

                //Create button and add it to grid
                Button b = ButtonNoHoverEffect(file.Image);
                b.Tag = file.Path;
                b.ToolTip = "Linker muisknop om te openen | Rechter muisknop om te verwijderen";
                b.Style = customButtonStyle;

                fileIDs.Add("file_" + fileID.ToString(), file.TargetName);
                b.Name = "file_" + fileID.ToString();
                fileID++;

                Grid containerGrid = new Grid();
                containerGrid.Children.Add(b);

                Image exclaim = MarkAsDeletable(file);
                containerGrid.Children.Add(exclaim);

                if (CheckIfDeletable(containerGrid, exclaim))
                {
                    b.MouseEnter += (sender, e) =>
                    {
                        exclaim.Source = SetIcon("Trash_Bin.png").ImageSource;
                    };

                    b.MouseLeave += (sender, e) =>
                    {
                        exclaim.Source = SetIcon("ExclamationMark.png").ImageSource;
                    };
                }

                RoutedEventHandler ClickOpenHandler = CreateClickOpenHandler(file);
                RoutedEventHandler ClickDelHandler = CreateClickDelHandler(file);

                folderGrid.AddHandler(Button.MouseLeftButtonDownEvent, ClickOpenHandler);
                folderGrid.AddHandler(Button.MouseLeftButtonDownEvent, ClickDelHandler);

                b.MouseRightButtonDown += ClickMarkHandler;

                b.Click += (sender, e) =>
                {
                    if (b.Tag is string filePath)
                    {
                        if (exclaim.Source != null)
                        {
                            if (exclaim.Source.ToString().Equals(SetIcon("Trash_Bin.png").ImageSource.ToString()))
                            {
                                ClickDelHandler?.Invoke(this, e);
                            }
                            else
                            {
                                ClickOpenHandler?.Invoke(this, e);
                            }
                        }
                        else
                        {
                            ClickOpenHandler?.Invoke(this, e);
                        }
                    }
                };

                folderGrid.Children.Add(containerGrid);

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

            DeleteButton = new ToggleButton
            {
                Content = "🗑️",
                FontSize = 35,
                Width = 58,
                Height = 58,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                Cursor = System.Windows.Input.Cursors.Hand,
                Style = (Style)System.Windows.Application.Current.TryFindResource("RoundButtonTheme"),
                Visibility = Visibility.Collapsed
            };

            DeleteButton.Click += OnDeleteButtonClick;

            folderPanel.Children.Add(DeleteButton);
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

        private RoutedEventHandler CreateClickOpenHandler(GenericFile file)
        {
            return (sender, args) =>
            {
                if (args.OriginalSource is Button clickedButton && clickedButton.Tag is string folderPath && file.TargetName is string fileName)
                {

                    string buttonFilePath = System.IO.Path.Combine(folderPath, fileName);

                    // Logic to run the file using the buttonFilePath
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
        }

        private RoutedEventHandler CreateClickDelHandler(GenericFile file)
        {
            return (sender, args) =>
            {
                if (args.OriginalSource is Button clickedButton && clickedButton.Tag is string folderPath && file.TargetName is string fileName)
                {

                    try
                    {
                        repository.DeleteData(folderPath, fileName);
                        UpdateOnChange();
                        System.Windows.MessageBox.Show($"Bestand/map is verwijderd");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error: {ex.Message}");
                    }

                }
            };
        }

        private void ClickMarkHandler(object sender, System.Windows.Input.MouseEventArgs args)
        {
            if (sender is Button button)
            {
                if (!DeletableButtons.Contains(button))
                {
                    // Add a semi-transparent overlay with a light red hue
                    var overlay = new Rectangle
                    {
                        Fill = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)),
                        Width = button.ActualWidth,
                        Height = button.ActualHeight
                    };

                    DeletableButtons.Add(button);
                    button.Content = overlay;
                }
                else
                {
                    DeletableButtons.Remove(button);
                    button.Content = null;
                }

                DeleteButton.Visibility = DeletableButtons.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (Button button in DeletableButtons) 
            { 
                if (button.Tag is string folderPath && button.Name is string ID)
                {
                    if (fileIDs.Contains(ID))
                    {
                        try
                        {
                            String fileName = fileIDs[ID].ToString();
                            repository.DeleteData(folderPath, fileName);
                        }
                        catch (Exception exc) 
                        { }
                    }
                }
            }

            UpdateOnChange();
            System.Windows.MessageBox.Show("Geselecteerde bestanden verwijderd");
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

        private Image MarkAsDeletable(GenericFile file)
        {
            Image iconImage = new Image();

            if (!File.Exists(file.Path + "\\" + file.TargetName) && !Directory.Exists(file.Path + "\\" + file.TargetName)) 
            {

                iconImage.Source = SetIcon("ExclamationMark.png").ImageSource;
                iconImage.Width = 30;
                iconImage.Height = 30;

                // Set the margin to position the image in the upper left corner of the button
                iconImage.Margin = new Thickness(20, 0, 0, 30);

            }

            return iconImage;
        }
        
        private bool CheckIfDeletable(Grid grid, Image image)
        {

            ImageSource rightSource = SetIcon("ExclamationMark.png").ImageSource;
            if (grid.Children.Contains(image))
            {
                if (image.Source == null)
                {
                    return false;
                }
                else if (image.Source.ToString().Equals(rightSource.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

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
                    bool result = DBConnector.Database.InsertFileData(path, ext);
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
                    bool result = DBConnector.Database.InsertFileData(path, "folder", "DirectoryIcon.png");
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