using StudySpark.Core.Grades;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    internal class GradesViewModel : INotifyPropertyChanged {
        public RelayCommand LogInAction { get; set; }
        public RelayCommand LogOutAction { get; set; }

        public event EventHandler? LoginButtonClicked;
        public event EventHandler? LogoutButtonClicked;

        private bool _isStudentLoggedIn;

        public ObservableCollection<GradeElement> GradeViewElements { get; } = new ObservableCollection<GradeElement>();

        public bool IsStudentLoggedIn {
            get { return _isStudentLoggedIn; }
            set {
                if (_isStudentLoggedIn != value) {
                    _isStudentLoggedIn = value;
                    OnPropertyChanged(nameof(IsStudentLoggedIn));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GradesViewModel() {
            LogInAction = new RelayCommand(o => HandleLogIn());
            LogOutAction = new RelayCommand(o => HandleLogOut());

            load();
        }

        private void load() {
            foreach (GradeElement gradeElement in DBConnector.Database.ReadGradesData()) {
                GradeViewElements.Add(gradeElement);
                Debug.WriteLine(gradeElement.ToString());
            }
        }

        private void HandleLogOut() {
            IsStudentLoggedIn = false;
            LogoutButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void HandleLogIn() {
            IsStudentLoggedIn = true;
            LoginButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}