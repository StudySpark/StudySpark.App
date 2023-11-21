using System.Collections.Generic;
using System.ComponentModel;
using StudySpark.Core.FileManager;

namespace StudySpark.GUI.WPF.MVVM.Model {
    class FilesModel : INotifyPropertyChanged  {

        public event PropertyChangedEventHandler? PropertyChanged;

        public int id { get; set; }
        public string path { get; set; }
        public string targetname { get; set; }
        public string type { get; set; }
        public string image { get; set; }

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
