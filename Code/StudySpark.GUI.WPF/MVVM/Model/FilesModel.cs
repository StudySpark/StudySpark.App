using System.Collections.Generic;
using System.ComponentModel;
using StudySpark.Core.FileManager;

namespace StudySpark.GUI.WPF.MVVM.Model {
    class FilesModel : INotifyPropertyChanged  {

        public event PropertyChangedEventHandler? PropertyChanged;

        private List<SearchFiles> files = new List<SearchFiles>();

        public List<SearchFiles> Files { get { return files; } }

        public void AddFile(SearchFiles file) {
            files.Add(file);
            OnPropertyChanged(nameof(Files));
        }

        public void RemoveFile(SearchFiles file) {
            files.Remove(file);
            OnPropertyChanged(nameof(Files));
        }

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
