using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    internal class LoginViewModel : ObservableObject {
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
        public RelayCommand LoginCommand { get; set; }
        public LoginViewModel() {
            LoginCommand = new RelayCommand(o => LoginUser());


        }
        public void LoginUser() {
            if (string.IsNullOrEmpty(username)) {
                MessageBox.Show("E-mail is vereist");
                return;
            }

            if (string.IsNullOrEmpty(password)) {
                MessageBox.Show("Wachtwoord is vereist");
                return;
            }

            DBConnector.Database.CreateUser(username, password);

        }

    }
}
