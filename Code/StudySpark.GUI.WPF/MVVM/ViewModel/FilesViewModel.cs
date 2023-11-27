using StudySpark.GUI.WPF.Core;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    internal class FilesViewModel : ObservableObject {
        public RelayCommand FilesSolutionCommand { get; set; }
        public RelayCommand FilesDownloadCommand { get; set; }

        public FilesSolutionViewModel FilesSolutionVM { get; set; }
        public FilesDownloadViewModel FilesDownloadVM { get; set; }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public FilesViewModel() {
            FilesSolutionVM = new FilesSolutionViewModel();
            FilesDownloadVM = new FilesDownloadViewModel();

            CurrentView = FilesSolutionVM;

            FilesSolutionCommand = new RelayCommand(o =>
            {
                CurrentView = FilesSolutionVM;
            });

            FilesDownloadCommand = new RelayCommand(o =>
            {
                CurrentView = FilesDownloadVM;
            });
        }
    }
}
