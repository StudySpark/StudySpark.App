using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class LoginViewModel : ObservableObject
    {
        private string username;

        public string Username { get { return username; } 
            set {
                    username = value;
                    OnPropertyChanged(nameof(Username));
                }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged(nameof(password));
            }
        }
        public RelayCommand LoginCommand { get; set; }
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(o => LoginUser());


        }
        public void LoginUser()
        {
            Debug.WriteLine($"user: '{username}'");
            Debug.WriteLine($"password: '{password}'");

            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) 
            {
                MessageBox.Show("A field is empty");
                return;
            }
            UserRepository userRepository = new UserRepository();
            userRepository.createUser(username, password);

        }
     
    }
}
