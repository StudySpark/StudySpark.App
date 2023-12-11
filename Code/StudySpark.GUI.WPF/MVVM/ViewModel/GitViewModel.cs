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
using LibGit2Sharp;
using System.Windows.Controls;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    public class GitViewModel : ObservableObject
    {
        private object _currentRepoList;

        public RelayCommand OpenRepoSelectCommand { get; private set; }

        WrapPanel repoPanel = new WrapPanel();

        public List<GenericGit> repos = new List<GenericGit>();

        private List<GenericGit> previousRepos;

        private ListView commitListView = new ListView(); // Add ListView

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

            foreach (GenericGit repo in repos)
            {
                // Display repositories as buttons (your existing code)
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

                //set row definitions for button and text
                Grid.SetRow(b, 0);
                Grid.SetRow(t, 0);

                Thickness margin = repoGrid.Margin;
                margin.Bottom = 75;
                margin.Right = margin.Left = 5;
                repoGrid.Margin = margin;

                // Add grid to panel
                repoPanel.Children.Add(repoGrid);

                // Create a new instance of ListView for each repository
                ListView commitListView = new ListView();
                commitListView.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#272537"));
                commitListView.Foreground = Brushes.White;

                // Display commit information in the ListView
                using (var gitRepo = new Repository($"{repo.Path}\\{repo.TargetName}"))
                {
                    // Check if there are any commits
                    if (gitRepo.Commits.Any())
                    {
                        foreach (var commit in gitRepo.Commits)
                        {
                            DisplayCommitInfo(commitListView, repo, commit);
                        }
                    }
                    else
                    {
                        // If there are no commits, display a message
                        var noCommitsMessage = new TextBlock
                        {
                            Text = "No commits in this repository.",
                            Foreground = Brushes.Gray,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        commitListView.Items.Add(noCommitsMessage);
                    }
                }

                Style itemStyle = new Style(typeof(ListViewItem));
                itemStyle.Setters.Add(new Setter(ListViewItem.BorderBrushProperty, Brushes.White));
                itemStyle.Setters.Add(new Setter(ListViewItem.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
                commitListView.ItemContainerStyle = itemStyle;
                // Add the ListView to the view after processing each repository
                repoPanel.Children.Add(commitListView);
            }
        }



        private void DisplayCommitInfo(ListView commitListView, GenericGit repo, Commit commit)
        {
            // Create ListViewItem to hold commit information
            var listViewItem = new ListViewItem();

            // Add subitems with commit information
            listViewItem.Content = new StackPanel();
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Repository: {repo.TargetName}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Commit ID: {commit.Id.Sha}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Author: {commit.Author.Name}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Changed Files: {GetChangedFiles(commit)}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Message: {commit.Message}" });

            // Add ListViewItem to the ListView
            commitListView.Items.Add(listViewItem);
        }

        private string GetChangedFiles(Commit commit)
        {
            return string.Join(", ", commit.Tree.Select(entry => entry.Path));
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
                    }
                    else
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