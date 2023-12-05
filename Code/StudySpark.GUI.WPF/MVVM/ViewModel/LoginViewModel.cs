using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class LoginViewModel
    {
        public string username;
        public string password;
        public RelayCommand LoginCommand { get; set; }
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(o => LoginUser());
        }
        public void LoginUser()
        {
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
