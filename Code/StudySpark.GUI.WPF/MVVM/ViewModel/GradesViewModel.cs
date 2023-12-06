using StudySpark.Core.Generic;
using StudySpark.Core.Grades;
using StudySpark.GUI.WPF.Core;
using StudySpark.WebScraper;
using StudySpark.WebScraper.Educator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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

            GenericUser? user = DBConnector.Database.GetUser();
            if (user == null) {
                return;
            }

            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.Username = user.Username;
            scraperOptions.Password = user.Password;
            scraperOptions.Debug = false;
            EducatorWebScraper webScraper = new EducatorWebScraper(scraperOptions);

            webScraper.Load();
            List<StudentGrade> grades = webScraper.FetchGrades();
            webScraper.CloseDriver();

            if (grades.Count == 0) {
                return;
            }

            DBConnector.Database.ClearGradesData();
            GradeViewElements.Clear();

            Debug.WriteLine("Grades: " + grades.Count);
            foreach (StudentGrade grade in grades) {
                GradeElement gradeElement = new GradeElement();
                gradeElement.CourseName = grade.CourseName;
                gradeElement.CourseCode = grade.CourseCode;
                gradeElement.TestDate = grade.TestDate;
                gradeElement.Semester = grade.Semester;
                gradeElement.ECs = grade.ECs;
                gradeElement.Grade = grade.Grade;

                Debug.WriteLine(grade.ToString());

                DBConnector.Database.InsertGrade(gradeElement);
                GradeViewElements.Add(gradeElement);
            }
        }
    }
}
