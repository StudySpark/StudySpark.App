using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    internal class MainViewModel : ObservableObject {

        public RelayCommand OverviewViewCommand { get; set; }
        public RelayCommand NotesViewCommand { get; set; }
        public RelayCommand FilesViewCommand { get; set; }
        public RelayCommand TimelineViewCommand { get; set; }
        public RelayCommand ScheduleViewCommand { get; set; }
        public RelayCommand OpenSettingsCommand { get; set; }

        public RelayCommand MinimizeCommand { get; private set; }
        public RelayCommand MaximizeCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }

        public OverviewViewModel OverviewVM { get; set; }
        public NotesViewModel NotesVM { get; set; }
        public FilesViewModel FilesVM { get; set; }
        public ScheduleViewModel ScheduleVM { get; set; }
        public TimelineViewModel TimelineVM { get; set; }

        private object _currentView;

        public object CurrentView {
            get { return _currentView; }
            set {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel() {
            MinimizeCommand = new RelayCommand(o => MinimizeWindow());
            MaximizeCommand = new RelayCommand(o => MaximizeWindow());
            CloseCommand = new RelayCommand(o => CloseWindow());

            OverviewVM = new OverviewViewModel();
            NotesVM = new NotesViewModel();
            FilesVM = new FilesViewModel();
            ScheduleVM = new ScheduleViewModel();
            TimelineVM = new TimelineViewModel();

            CurrentView = OverviewVM;

            OverviewViewCommand = new RelayCommand(o => {
                CurrentView = OverviewVM;
            });

            NotesViewCommand = new RelayCommand(o => {
                CurrentView = NotesVM;
            });

            FilesViewCommand = new RelayCommand(o => {
                CurrentView = FilesVM;
            });

            TimelineViewCommand = new RelayCommand(o => {
                CurrentView = TimelineVM;
            });

            ScheduleViewCommand = new RelayCommand(o => {
                CurrentView = ScheduleVM;
            });

            OpenSettingsCommand = new RelayCommand(o => {
                Debug.WriteLine("Settings!");
            });

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
            if (Application.Current.MainWindow != null) {
                Application.Current.MainWindow.Close();
            }
        }
    }
}