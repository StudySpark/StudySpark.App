using StudySpark.Core;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Application = System.Windows.Application;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    internal class MainViewModel : ObservableObject {

        public RelayCommand OverviewViewCommand { get; set; }
        public RelayCommand NotesViewCommand { get; set; }
        public RelayCommand FilesViewCommand { get; set; }
        public RelayCommand TimelineViewCommand { get; set; }
        public RelayCommand ScheduleViewCommand { get; set; }
        public RelayCommand GradesViewCommand { get; set; }
        public RelayCommand GitViewCommand { get; set; }
        public RelayCommand BierViewCommand { get; set; }

        public RelayCommand MinimizeCommand { get; private set; }
        public RelayCommand MaximizeCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }

        private object? _currentView;
        public object? CurrentView {
            get { return _currentView; }
            set {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel() {
            Logger.Info("Starting application.");

            MinimizeCommand = new RelayCommand(o => MinimizeWindow());
            MaximizeCommand = new RelayCommand(o => MaximizeWindow());
            CloseCommand = new RelayCommand(o => CloseWindow());

            MainViewManager.CurrentMainViewEvent += ViewChangeEvent;

            CurrentView = MainViewManager.OverviewVM;

            OverviewViewCommand = new RelayCommand(o => {
                CurrentView = MainViewManager.OverviewVM;
            });

            NotesViewCommand = new RelayCommand(o => {
                CurrentView = MainViewManager.NotesVM;
            });

            FilesViewCommand = new RelayCommand(o => {
                CurrentView = MainViewManager.FilesVM;
            });

            TimelineViewCommand = new RelayCommand(o => {
                CurrentView = MainViewManager.TimelineVM;
            });

            ScheduleViewCommand = new RelayCommand(o => {
                CurrentView = MainViewManager.ScheduleVM;
            });

            GradesViewCommand = new RelayCommand(o => {
                CurrentView = MainViewManager.GradesVM;
            });

            GitViewCommand = new RelayCommand(o => {
                CurrentView = MainViewManager.GitVM;
            });

            BierViewCommand = new RelayCommand(o =>
            {
                CurrentView = MainViewManager.BierVM;
            });
                
        }

        private void ViewChangeEvent(object? sender, EventArgs e) {
            if (CurrentView == MainViewManager.CurrentMainView) {
                return;
            }

            CurrentView = MainViewManager.CurrentMainView;
        }

        private void MinimizeWindow() {
            if (Application.Current.MainWindow != null) {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            }
        }

        private void MaximizeWindow() {
            if (Application.Current.MainWindow != null) {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized) {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                } else {
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;

                    // Set the maximum size of the window to the screen dimensions
                    Application.Current.MainWindow.MaxWidth = SystemParameters.WorkArea.Width;
                    Application.Current.MainWindow.MaxHeight = SystemParameters.WorkArea.Height;
                }
            }
        }
        private void CloseWindow() {
            Logger.Info("Stopping application.");

            if (Application.Current.MainWindow != null) {
                Application.Current.MainWindow.Close();
            }
        }
    }
}