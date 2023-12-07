using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using StudySpark.Core.FileManager;
using StudySpark.Core.Generic;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    public class GitViewModel : ObservableObject
    {
        private object _currentRepoList;

        public RelayCommand OpenRepoSelectCommand { get; private set; }

        WrapPanel repoPanel = new WrapPanel();

        public List<GenericGit> repos = new List<GenericGit>();

        private List<GenericGit> previousRepos;

        public object CurrentRepoList
        {
            get
            {
                return _currentRepoList;
            }
            set
            {
                _currentRepoList = value;
            }
        }

        public GitViewModel()
        {
            OpenRepoSelectCommand = new RelayCommand(o => SelectRepository());

            UpdateOnChange();

            //set alignment for panel
            repoPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            repoPanel.VerticalAlignment = VerticalAlignment.Top;

            _currentRepoList = repoPanel;

        }

        private void UpdateOnChange()
        {
            previousRepos = repos;
            repos = DBConnector.Database.ReadGitData();
            repoPanel.Children.Clear();

            List<GenericGit> difference = repos.Except(previousRepos).ToList();
            foreach (GenericGit repo in difference)
            {
                Style customButtonStyle = (Style)System.Windows.Application.Current.TryFindResource("FileButtonTheme");

                Grid repoGrid = new Grid();
                repoGrid.RowDefinitions.Add(new RowDefinition());
                repoGrid.RowDefinitions.Add(new RowDefinition());

                //Create button and add it to grid
                System.Windows.Controls.Button b = ButtonNoHoverEffect();
                b.Tag = repo.Path;
                b.Style = customButtonStyle;
                repoGrid.Children.Add(b);

                //Create textbox and add it to grid
                TextBlock t = SubText();
                if (repo.TargetName.Length > 0)
                {
                    t.Text = TruncateFileName(repo.TargetName, 15);
                }
                else { t.Text = repo.Path; }
                t.ToolTip = repo.Path;
                repoGrid.Children.Add(t);

                //set row defenitions for button and text
                Grid.SetRow(b, 0);
                Grid.SetRow(t, 0);

                //
                Thickness margin = repoGrid.Margin;
                margin.Bottom = 75;
                margin.Right = margin.Left = 5;
                repoGrid.Margin = margin;

                //add grid to panel
                repoPanel.Children.Add(repoGrid);
            }
        }

        public System.Windows.Controls.Button ButtonNoHoverEffect()
        {
            System.Windows.Controls.Button button = new System.Windows.Controls.Button();

            button.Width = 200;
            button.Height = 50;
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22202f")); button.BorderThickness = new Thickness(0, 0, 0, 0);
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
        private void SelectRepository()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                    string path = folderBrowserDialog.SelectedPath;
                if (!string.IsNullOrEmpty(path))
                {
                    if (Directory.Exists(Path.Combine(path, ".git")))
                    {
                        bool result = DBConnector.Database.InsertGitData(path, "repository");
                        if (!result)
                        {
                            System.Windows.MessageBox.Show("Er is iets fout gegaan!");
                            return;
                        }
                        UpdateOnChange();
                        System.Windows.MessageBox.Show("Map succesvol toegevoegd!");
                    } else
                    {
                        System.Windows.MessageBox.Show("De geselecteerde map bevat geen repository");
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Er is iets fout gegaan!");
            }
        }
    }
}
