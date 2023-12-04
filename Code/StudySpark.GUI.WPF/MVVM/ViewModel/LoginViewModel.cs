using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    internal class LoginViewModel
    {
        public RelayCommand LoginCommand { get; set; }
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(o => LoginUser());
        }
        public void LoginUser()
        {

        }
    }
}
