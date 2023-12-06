using StudySpark.Core.Grades;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class GradesViewModel : INotifyPropertyChanged {
        public RelayCommand EducatorLoginCommand { get; set; }


        public ObservableCollection<GradeElement> GradeViewElements { get; } = new ObservableCollection<GradeElement>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GradesViewModel() {
            load();

            EducatorLoginCommand = new RelayCommand(o => {
                MainViewManager.CurrentMainView = MainViewManager.LoginVM;
                //CurrentView = LoginVM;
            });
        }

        private void load() {
            foreach (GradeElement gradeElement in DBConnector.Database.ReadGradesData()) {
                GradeViewElements.Add(gradeElement);
            }
        }
    }
}
