using StudySpark.Core.Generic;
using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using StudySpark.GUI.WPF.MVVM.View;
using StudySpark.WebScraper;
using StudySpark.WebScraper.Educator;
using StudySpark.WebScraper.WIP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static StudySpark.GUI.WPF.Core.LoginViewEventArgs;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class LoginViewModel : ObservableObject {
        public delegate void LoginViewEventHandler(object sender, LoginViewEventArgs e);
        public static event LoginViewEventHandler? ViewDataChangeEvent;
        public static event EventHandler? FormResetErrorsEvent, FormMissingEmailEvent, FormMissingPasswordEvent, FormMissingTwoFAEvent;

        public enum RETURNVIEW {
            EDUCATOR,
            WIP
        }

        public static RETURNVIEW ReturnToView;

        private string username;

        public string Username {
            get { return username; }
            set {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string password;

        public string Password {
            get { return password; }
            set {
                password = value;
                OnPropertyChanged(nameof(password));
            }
        }

        private string twoFA;

        public string TwoFA {
            get { return twoFA; } 
            set { 
                twoFA = value;
                OnPropertyChanged(nameof(TwoFA));
            }
        }

        public RelayCommand LoginCommand { get; set; }
        public LoginViewModel() {
            LoginCommand = new RelayCommand(o => LoginUser());

            LoginView.LoginSuccessEvent += LoginView_LoginSuccessEvent;

            GenericUser? user = DBConnector.Database.GetUser();
            if (user != null) {
                Username = user.Username;
                Password = user.Password;
                
            }
        }

        private void LoginView_LoginSuccessEvent(object? sender, EventArgs e) {
            if (ReturnToView == RETURNVIEW.EDUCATOR) {
                MainViewManager.CurrentMainView = MainViewManager.GradesVM;
            } else {
                MainViewManager.CurrentMainView = MainViewManager.ScheduleVM;
            }
        }

        public void LoginUser() {
            FormResetErrorsEvent?.Invoke(this, EventArgs.Empty);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(twoFA)) {
                if (string.IsNullOrEmpty(username)) {
                    FormMissingEmailEvent?.Invoke(this, EventArgs.Empty);
                }

                if (string.IsNullOrEmpty(password)) {
                    FormMissingPasswordEvent?.Invoke(this, EventArgs.Empty);
                }

                if (string.IsNullOrEmpty(twoFA))
                {
                    FormMissingTwoFAEvent?.Invoke(this, EventArgs.Empty);
                }
                return;
            }

            DBConnector.Database.CreateUser(username, password, twoFA);

            LoginViewEventArgs eventArgs = new LoginViewEventArgs();
            eventArgs.LoginViewEventType = LoginViewEvent.USERDATASUBMITTED;
            ViewDataChangeEvent?.Invoke(this, eventArgs);

            // Thread.Sleep(1000);

            Thread thread = new Thread(TestLoginCredentialsThread);
            thread.Start();
        }

        private void TestLoginCredentialsThread() {

            LoginViewEventArgs eventArgs = new LoginViewEventArgs();
            eventArgs.LoginViewEventType = TestLoginCredentials(username, password, twoFA) ? LoginViewEvent.LOGINSUCCESS : LoginViewEvent.LOGINFAILED;
            try {
                Application.Current.Dispatcher.Invoke(() => {
                    ViewDataChangeEvent?.Invoke(this, eventArgs);
                });
            } catch (Exception) { }
        }

        public bool TestLoginCredentials(string username, string password, string twoFA) {
            ScraperOptions scraperOptions = new ScraperOptions();
            scraperOptions.Username = username;
            scraperOptions.Password = password;
            scraperOptions.TwoFACode = twoFA;
            scraperOptions.Debug = false;

            if (ReturnToView == RETURNVIEW.EDUCATOR) {
                EducatorWebScraper webScraper = new EducatorWebScraper(scraperOptions);

                webScraper.SetupDriver();
                bool result = webScraper.TestLoginCredentials();

                webScraper.CloseDriver();
                return result;
            } else {
                WIPWebScraper webScraper = new WIPWebScraper(scraperOptions);

                webScraper.SetupDriver();
                bool result = webScraper.TestLoginCredentials();

                webScraper.CloseDriver();
                return result;
            }
        }
    }
}