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
using System.Text.RegularExpressions;
using System.Windows.Input;
using Orientation = System.Windows.Controls.Orientation;
using System.Windows.Controls.Primitives;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    public class GitViewModel : ObservableObject
    {
        private object _currentRepoList;

        public RelayCommand OpenRepoSelectCommand { get; private set; }

        WrapPanel repoPanel = new WrapPanel();

        public List<GenericGit> repos = new List<GenericGit>();

        private List<GenericGit> previousRepos;

        private ListView commitListView = new ListView();

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
        private int _maxCommitsToShow = 10;
        public int MaxCommitsToShow
        {
            get { return _maxCommitsToShow; }
            set
            {
                if (_maxCommitsToShow != value)
                {
                    _maxCommitsToShow = value;
                    OnPropertyChanged(nameof(MaxCommitsToShow));
                    UpdateOnChange();
                }
            }
        }
        private int _maxCommitsForCurrentRepo;
        public int MaxCommitsForCurrentRepo
        {
            get { return _maxCommitsForCurrentRepo; }
            set
            {
                if (_maxCommitsForCurrentRepo != value)
                {
                    _maxCommitsForCurrentRepo = value;
                    OnPropertyChanged(nameof(MaxCommitsForCurrentRepo));
                }
            }
        }
        private bool _isVerboseView;
        public bool IsVerboseView
        {
            get { return _isVerboseView; }
            set
            {
                if (_isVerboseView != value)
                {
                    _isVerboseView = value;
                    OnPropertyChanged(nameof(IsVerboseView));
                    UpdateOnChange();
                }
            }
        }

        public GitViewModel()
        {
            OpenRepoSelectCommand = new RelayCommand(o => SelectRepository());

            

            //set alignment for panel
            repoPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            repoPanel.VerticalAlignment = VerticalAlignment.Top;

            _currentRepoList = repoPanel;
            UpdateOnChange();
        }

        private void UpdateOnChange()
        {
            previousRepos = repos;
            repos = DBConnector.Database.ReadGitData();
            repoPanel.Children.Clear();
            repoPanel.Orientation = Orientation.Vertical;

            UniformGrid uniformGrid = new UniformGrid();
            uniformGrid.Columns = 2;

            foreach (GenericGit repo in repos)
            {


                // Create a new instance of ListView for each repository
                ListView commitListView = new ListView();
                commitListView.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#272537"));
                commitListView.Foreground = Brushes.White;
                commitListView.MinWidth = 550;
                // Set the margin to create a small gap between ListView elements
                commitListView.Margin = new Thickness(0, 0, 0, 10);

                // Display commit information in the ListView
                using (var gitRepo = new Repository($"{repo.Path}\\{repo.TargetName}"))
                {
                    // Check if there are any commits
                    if (gitRepo.Commits.Any())
                    {
                        int totalCommits = gitRepo.Commits.Count();
                        int commitsToShow = Math.Min(MaxCommitsToShow, gitRepo.Commits.Count());
                        if (MaxCommitsForCurrentRepo < totalCommits) { MaxCommitsForCurrentRepo = totalCommits; }
                        for (int i = 0; i < commitsToShow; i++)
                        {
                            if (IsVerboseView)
                            {
                                // Display verbose commit information
                                DisplayVerboseCommitInfo(commitListView, repo, gitRepo.Commits.ElementAt(i));
                            }
                            else
                            {
                                // Display brief commit information
                                DisplayBriefCommitInfo(commitListView, repo, gitRepo.Commits.ElementAt(i));
                            }
                        }
                    }
                    else
                    {
                        // If there are no commits, display a message
                        var noCommitsMessage = new TextBlock
                        {
                            Text = $"No commits in this repository ({repo.TargetName}).",
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

                // Add the repository bar and commitListView to the UniformGrid
                //uniformGrid.Children.Add(repositoryBar);
                uniformGrid.Children.Add(commitListView);
            }

            // Set the UniformGrid as the CurrentRepoList
            _currentRepoList = uniformGrid;
            OnPropertyChanged(nameof(CurrentRepoList));
        }





        private void DisplayBriefCommitInfo(ListView commitListView, GenericGit repo, Commit commit)
        {
            // Create ListViewItem to hold brief commit information
            var listViewItem = new ListViewItem();

            // Add subitems with brief commit information
            listViewItem.Content = new StackPanel();
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Repository: {repo.TargetName}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Auteur: {commit.Author.Name}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Datum: {commit.Author.When}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"+/-: {GetChangedFilesCount($"{repo.Path}\\{repo.TargetName}", commit)}" });

            // Add ListViewItem to the ListView
            commitListView.Items.Add(listViewItem);
        }

        private void DisplayVerboseCommitInfo(ListView commitListView, GenericGit repo, Commit commit)
        {
            // Create ListViewItem to hold verbose commit information
            var listViewItem = new ListViewItem();

            // Add subitems with verbose commit information
            listViewItem.Content = new StackPanel();
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Repository: {repo.TargetName}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Commit ID: {commit.Id.Sha}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Auteur: {commit.Author.Name}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Bestanden: {GetChangedFiles(commit)}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"+/-: {GetChangedFilesCount($"{repo.Path}\\{repo.TargetName}", commit)}" });
            (listViewItem.Content as StackPanel).Children.Add(new TextBlock { Text = $"Bericht: {commit.Message}" });

            // Add ListViewItem to the ListView
            commitListView.Items.Add(listViewItem);
        }


        private string GetChangedFilesCount(string a, Commit commit)
        {
            using (var repo = new Repository(a))
            {


                int totalAdditions = 0;
                int totalDeletions = 0;

                foreach (var parent in commit.Parents)
                {
                    TreeChanges changes = repo.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree);

                    foreach (TreeEntryChanges change in changes)
                    {
                        if (change.Status != ChangeKind.Deleted)
                        {
                            Patch patch = repo.Diff.Compare<Patch>(parent.Tree, commit.Tree, new[] { change.Path });
                            totalAdditions += patch.LinesAdded;
                        }

                        if (change.Status != ChangeKind.Added)
                        {
                            Patch patch = repo.Diff.Compare<Patch>(parent.Tree, commit.Tree, new[] { change.Path });
                            totalDeletions += patch.LinesDeleted;
                        }
                    }
                }


                return $"+{totalAdditions} || -{totalDeletions}";
            }
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