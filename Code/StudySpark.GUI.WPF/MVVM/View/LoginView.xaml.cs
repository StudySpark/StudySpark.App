using StudySpark.GUI.WPF.Core;
using StudySpark.GUI.WPF.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static StudySpark.GUI.WPF.Core.LoginViewEventArgs;
using static StudySpark.GUI.WPF.MVVM.ViewModel.LoginViewModel;

namespace StudySpark.GUI.WPF.MVVM.View {
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : UserControl {

        public static event EventHandler? LoginSuccessEvent;

        public LoginView() {
            InitializeComponent();

            LoginViewModel.ViewDataChangeEvent += OnViewChangeEvent;


            LoginGrid.Visibility = Visibility.Visible;
            TestConnection.Visibility = Visibility.Hidden;
            LoginFailed.Visibility = Visibility.Hidden;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            passwordBox.Password = (sender as TextBox).Text;
        }

        private void OnViewChangeEvent(object? sender, LoginViewEventArgs ea) {
            switch (ea.LoginViewEventType) {
                case LoginViewEvent.USERDATASUBMITTED:
                    LoginGrid.Visibility = Visibility.Hidden;
                    TestConnection.Visibility = Visibility.Visible;
                    LoginFailed.Visibility = Visibility.Hidden;
                    break;

                case LoginViewEvent.LOGINSUCCESS:
                    LoginGrid.Visibility = Visibility.Hidden;
                    TestConnection.Visibility = Visibility.Hidden;
                    LoginFailed.Visibility = Visibility.Hidden;

                    LoginSuccessEvent?.Invoke(this, EventArgs.Empty);
                    break;

                case LoginViewEvent.LOGINFAILED:
                    LoginGrid.Visibility = Visibility.Visible;
                    TestConnection.Visibility = Visibility.Hidden;
                    LoginFailed.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Button_HelpEmail_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Vul hier je Windesheim account e-mail in");
        }

        private void Button_HelpPassword_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Vul hier je Windesheim account wachtwoord in");
        }
    }
}
