using StudySpark.GUI.WPF.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace StudySpark.GUI.WPF.MVVM.View {
    /// <summary>
    /// Interaction logic for GradesView.xaml
    /// </summary>
    public partial class GradesView : UserControl {

        public GradesView() {
            InitializeComponent();

            Loaded += GradesView_Loaded;
            Unloaded += GradesView_Unloaded;
        }

        private void GradesView_Loaded(object sender, RoutedEventArgs e) {
            LoadingMessage.Visibility = Visibility.Visible;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            GradesItemControl.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
            Missing2FACodeMessage.Visibility = Visibility.Collapsed;
            EducatorLoadMessage.Visibility = Visibility.Collapsed;

            if (DataContext is GradesViewModel viewModel) {
                viewModel.NoUserLoggedInEvent += OnNoUserLoggedInEvent;
                viewModel.InvalidUserCredentialsEvent += OnUserInvalidCredentialsEvent;
                viewModel.Missing2FACodeEvent += OnMissing2FACodeEvent;
                viewModel.GradesLoadedEvent += OnGradesLoaded;
                viewModel.EducatorLoadStartedEvent += OnEducatorLoadStartedEvent;
                viewModel.EducatorLoadFinishedEvent += OnEducatorLoadFinishedEvent;
                viewModel.IsViewLoaded = true;
            }

            Loaded -= GradesView_Loaded;
        }

        private void GradesView_Unloaded(object sender, RoutedEventArgs e) {
            if (DataContext is GradesViewModel viewModel) {
                viewModel.IsViewLoaded = false;
            }
        }

        private void OnNoUserLoggedInEvent(object? sender, EventArgs e) {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Visible;
            GradesItemControl.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
            Missing2FACodeMessage.Visibility = Visibility.Collapsed;
        }

        private void OnUserInvalidCredentialsEvent(object? sender, EventArgs e) {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            GradesItemControl.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Visible;
            Missing2FACodeMessage.Visibility = Visibility.Collapsed;
        }

        private void OnMissing2FACodeEvent(object? sender, EventArgs e) {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            GradesItemControl.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
            Missing2FACodeMessage.Visibility = Visibility.Visible;
        }

        private void OnGradesLoaded(object? sender, EventArgs e) {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            GradesItemControl.Visibility = Visibility.Visible;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
            Missing2FACodeMessage.Visibility = Visibility.Collapsed;
        }

        private void OnEducatorLoadStartedEvent(object? sender, EventArgs e) {
            EducatorLoadMessage.Visibility = Visibility.Visible;
        }

        private void OnEducatorLoadFinishedEvent(object? sender, EventArgs e) {
            EducatorLoadMessage.Visibility = Visibility.Collapsed;
        }
    }
}