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
    public class ScheduleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? NoUserLoggedInEvent, InvalidUserCredentialsEvent, Missing2FACodeEvent, ScheduleLoadedEvent, WIPLoadStartedEvent, WIPLoadFinishedEvent;

        public ObservableCollection<GradeElement> ScheduleViewElements { get; } = new ObservableCollection<GradeElement>();

        public RelayCommand ScheduleLoginCommand { get; set; }

        private bool isUserLoggedIn = false, isUserCredentialsValid = false;

        private bool isViewLoaded;
        public bool IsViewLoaded
        {
            get { return isViewLoaded; }
            set
            {
                if (isViewLoaded != value)
                {
                    isViewLoaded = value;
                    OnPropertyChanged(nameof(IsViewLoaded));

                    // Check and raise the event when the view is loaded
                    if (isViewLoaded && !isUserLoggedIn)
                    {
                        NoUserLoggedInEvent?.Invoke(null, EventArgs.Empty);
                    }
                }
            }
        }

        public ScheduleViewModel() 
        {
            preload();

            ScheduleLoginCommand = new RelayCommand(o => {
                LoginViewModel.ReturnToView = LoginViewModel.RETURNVIEW.WIP;
                MainViewManager.CurrentMainView = MainViewManager.LoginVM;
                //CurrentView = LoginVM;
            });

        }

        private void preload()
        {
            ScheduleViewElements.Clear();

            GenericUser? user = DBConnector.Database.GetUser();
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                isUserLoggedIn = false;
                return;
            }
            isUserLoggedIn = true;

            Thread thread = new Thread(TestLoginCredentialsThread);
            thread.Start(user);
        }

        private void load(GenericUser user, bool loginResult)
        {
            if (user.TwoFA == null || user.TwoFA.Length == 0)
            {
                Missing2FACodeEvent?.Invoke(null, EventArgs.Empty);
                return;
            }

            if (!loginResult)
            {
                InvalidUserCredentialsEvent?.Invoke(null, EventArgs.Empty);
                return;
            }

            ScheduleLoadedEvent?.Invoke(null, EventArgs.Empty);

            WIPLoadStartedEvent?.Invoke(null, EventArgs.Empty);
            Thread thread = new Thread(LoadUpdatedSchedule);
            thread.Start(user);
        }

        private void LoadUpdatedSchedule(object? parameters) 
        { 
            if (parameters == null)
            {
                return;
            }

            GenericUser? user = (GenericUser)parameters;

            if (user.TwoFA == null || user.TwoFA.Length == 0)
            {
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

        private void TestLoginCredentialsThread(object? parameters)
        {
            if (parameters == null)
            {
                return;
            }

            GenericUser user = (GenericUser)parameters;

            bool result = TestLoginCredentials(user.Username, user.Password);
            try
            {
                Application.Current.Dispatcher.Invoke(() => {
                    load(user, result);
                });
            }
            catch (Exception) { }
        }

        public bool TestLoginCredentials(string username, string password)
        {
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
