using StudySpark.Core.Generic;
using StudySpark.Core.Grades;
using StudySpark.GUI.WPF.Core;
using StudySpark.WebScraper.WIP;
using StudySpark.WebScraper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static StudySpark.GUI.WPF.Core.LoginViewEventArgs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Diagnostics;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {

    public class ScheduleItemElement {
        public string? StartEndTime {  get; set; }
        public string? Classroom { get; set; }
        public string? CourseName { get; set; }
        public string? Class { get; set; }
        public string? Teacher { get; set; }
        public string? CourseCode { get; set; }
        public string? ElementMargin { get; set; }
    }

public class ScheduleViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? NoUserLoggedInEvent, InvalidUserCredentialsEvent, Missing2FACodeEvent, ScheduleLoadedEvent, WIPLoadStartedEvent, WIPLoadFinishedEvent;

    public ObservableCollection<GradeElement> ScheduleViewElements { get; } = new ObservableCollection<GradeElement>();

    public ObservableCollection<ScheduleItemElement> ScheduleMondayItemElements { get; } = new ObservableCollection<ScheduleItemElement>(); // TODO: Object
    public ObservableCollection<ScheduleItemElement> ScheduleTuesdayItemElements { get; } = new ObservableCollection<ScheduleItemElement>(); // TODO: Object
    public ObservableCollection<ScheduleItemElement> ScheduleWednesdayItemElements { get; } = new ObservableCollection<ScheduleItemElement>(); // TODO: Object
    public ObservableCollection<ScheduleItemElement> ScheduleThursdayItemElements { get; } = new ObservableCollection<ScheduleItemElement>(); // TODO: Object
    public ObservableCollection<ScheduleItemElement> ScheduleFridayItemElements { get; } = new ObservableCollection<ScheduleItemElement>(); // TODO: Object

    public RelayCommand ScheduleLoginCommand { get; set; }

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

    public ScheduleViewModel() {
        preload();

        ScheduleLoginCommand = new RelayCommand(o => {
            LoginViewModel.ReturnToView = LoginViewModel.RETURNVIEW.WIP;
            MainViewManager.CurrentMainView = MainViewManager.LoginVM;
            //CurrentView = LoginVM;
        });

            ScheduleMondayItemElements.Add(new ScheduleItemElement() {
                StartEndTime = "08:30-10:30",
                Classroom = "T1.23",
                CourseName = "Test1",
                Class = "ICTTest1",
                Teacher = "Me",
                CourseCode = "ABC | 123",
                ElementMargin = "3,20,3,0"
            });

            ScheduleMondayItemElements.Add(new ScheduleItemElement() {
                StartEndTime = "11:30-12:30",
                Classroom = "T1.23",
                CourseName = "Test2",
                Class = "ICTTest2",
                Teacher = "Me",
                CourseCode = "ABC | 123",
                ElementMargin = "3,20,3,0"
            });

            ScheduleTuesdayItemElements.Add(new ScheduleItemElement() {
                StartEndTime = "12:30-13:30",
                Classroom = "T1.23",
                CourseName = "Test3",
                Class = "ICTTest3",
                Teacher = "Me",
                CourseCode = "ABC | 123",
                ElementMargin = "3,150,3,0"
            });

        }

    private void preload() {
        ScheduleViewElements.Clear();

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

        ScheduleLoadedEvent?.Invoke(null, EventArgs.Empty);

        WIPLoadStartedEvent?.Invoke(null, EventArgs.Empty);
        Thread thread = new Thread(LoadUpdatedSchedule);
        thread.Start(user);
    }

    private void LoadUpdatedSchedule(object? parameters) {
        if (parameters == null) {
            return;
        }

        GenericUser? user = (GenericUser)parameters;

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
        WIPWebScraper scraper = new WIPWebScraper(scraperOptions);

        scraper.Load();
        scraper.FetchSchedule();
        scraper.CloseDriver();

    }

    private void TestLoginCredentialsThread(object? parameters) {
        if (parameters == null) {
            return;
        }

        GenericUser user = (GenericUser)parameters;

        bool result = TestLoginCredentials(user.Username, user.Password);
        try {
            Application.Current.Dispatcher.Invoke(() => {
                load(user, result);
            });
        } catch (Exception) { }
    }

    public bool TestLoginCredentials(string username, string password) {
        ScraperOptions scraperOptions = new ScraperOptions();
        scraperOptions.Username = username;
        scraperOptions.Password = password;
        scraperOptions.Debug = false;

        //EducatorWebScraper webScraper = new EducatorWebScraper(scraperOptions);
        WIPWebScraper webScraper = new WIPWebScraper(scraperOptions);

        webScraper.SetupDriver();
        bool result = webScraper.TestLoginCredentials();

        webScraper.CloseDriver();
        return result;
    }

    private void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
}
