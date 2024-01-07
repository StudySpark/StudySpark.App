using Microsoft.VisualBasic.ApplicationServices;
using StudySpark.Core.Generic;
using StudySpark.Core.Grades;
using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using StudySpark.WebScraper;
using StudySpark.WebScraper.Educator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using static StudySpark.GUI.WPF.Core.LoginViewEventArgs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class GradesViewModel : INotifyPropertyChanged {
        public RelayCommand EducatorLoginCommand { get; set; }
        public event EventHandler? NoUserLoggedInEvent, InvalidUserCredentialsEvent, Missing2FACodeEvent, GradesLoadedEvent, EducatorLoadStartedEvent, EducatorLoadFinishedEvent;

        private bool isUserLoggedIn = false, isUserCredentialsValid = false;

        private bool isViewLoaded;
        public bool IsViewLoaded {
            get { return isViewLoaded; }
            set {
                if (isViewLoaded != value) {
                    isViewLoaded = value;
                    OnPropertyChanged(nameof(IsViewLoaded));

                    // Check and raise the event when the view is loaded
                    if (isViewLoaded && !isUserLoggedIn) {
                        NoUserLoggedInEvent?.Invoke(null, EventArgs.Empty);
                    }
                }
            }
        }


        public ObservableCollection<GradeElement> GradeViewElements { get; } = new ObservableCollection<GradeElement>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GradesViewModel() {
            preload();

            EducatorLoginCommand = new RelayCommand(o => {
                LoginViewModel.ReturnToView = LoginViewModel.RETURNVIEW.EDUCATOR;
                MainViewManager.CurrentMainView = MainViewManager.LoginVM;
                //CurrentView = LoginVM;
            });
        }

        private void preload() {
            GradeViewElements.Clear();

            GenericUser? user = DBConnector.Database.GetUser();
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password)) {
                isUserLoggedIn = false;
                return;
            }
            isUserLoggedIn = true;

            Thread thread = new Thread(TestLoginCredentialsThread);
            thread.Start(user);
        }

        private void load(GenericUser user, bool loginResult) {
            if (user.TwoFA == null || user.TwoFA.Length == 0) {
                Missing2FACodeEvent?.Invoke(null, EventArgs.Empty);
                return;
            }

            if (!loginResult) {
                InvalidUserCredentialsEvent?.Invoke(null, EventArgs.Empty);
                return;
            }

            List<GradeElement> cachedGrades = DBConnector.Database.ReadGradesData();
            foreach (GradeElement gradeElement in cachedGrades) {
                GradeViewElements.Add(gradeElement);
            }

            if (cachedGrades.Count > 0) {
                GradesLoadedEvent?.Invoke(null, EventArgs.Empty);
            }

            EducatorLoadStartedEvent?.Invoke(null, EventArgs.Empty);
            Thread thread = new Thread(LoadEducatorData);
            thread.Start(user);
        }

        private void LoadEducatorData(object? parameters) {
            if (parameters == null) {
                return;
            }

            GenericUser user = (GenericUser)parameters;

            if (user.TwoFA == null || user.TwoFA.Length == 0) {
                Application.Current.Dispatcher.Invoke(() => {
                    Missing2FACodeEvent?.Invoke(null, EventArgs.Empty);
                }); 

                return;
            }

            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.Username = user.Username;
            scraperOptions.Password = user.Password;
            scraperOptions.TwoFACode = user.TwoFA;
            scraperOptions.Debug = false;
            EducatorWebScraper webScraper = new EducatorWebScraper(scraperOptions);

            DBRepository.InvalidateUser2FACode();

            webScraper.Load();
            List<StudentGrade> grades = webScraper.FetchGrades();
            webScraper.CloseDriver();

            if (grades != null && grades.Count != 0) {
                try {
                    Application.Current.Dispatcher.Invoke(() => {
                        showNewEducatorData(grades);
                    });
                } catch (NullReferenceException) { }
            } else {
                Application.Current.Dispatcher.Invoke(() => {
                    Missing2FACodeEvent?.Invoke(null, EventArgs.Empty);
                });
            }
        }

        private void showNewEducatorData(List<StudentGrade> grades) {
            EducatorLoadFinishedEvent?.Invoke(null, EventArgs.Empty);

            DBConnector.Database.ClearGradesData();
            GradeViewElements.Clear();

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

            GradesLoadedEvent?.Invoke(null, EventArgs.Empty);
        }

        private void TestLoginCredentialsThread(object? parameters) {
            if (parameters == null) {
                return;
            }

            GenericUser user = (GenericUser)parameters;

            bool loginResult = TestLoginCredentials(user.Username, user.Password);
            try {
                Application.Current.Dispatcher.Invoke(() => {
                    load(user, loginResult);
                });
            } catch (NullReferenceException) { }

        }

        public bool TestLoginCredentials(string username, string password) {
            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.Username = username;
            scraperOptions.Password = password;
            scraperOptions.Debug = false;

            EducatorWebScraper webScraper = new EducatorWebScraper(scraperOptions);

            webScraper.SetupDriver();
            bool result = webScraper.TestLoginCredentials();

            webScraper.CloseDriver();
            return result;
        }
    }
}