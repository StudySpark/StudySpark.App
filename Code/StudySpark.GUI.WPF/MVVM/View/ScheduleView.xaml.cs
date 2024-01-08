using StudySpark.Core.Grades;
using StudySpark.GUI.WPF.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : UserControl {

        public ScheduleView() {
            InitializeComponent();

            Loaded += ScheduleView_Loaded;
            Unloaded += ScheduleView_Unloaded;

            //CreateGrid();
        }

        private void ScheduleView_Loaded(object sender, RoutedEventArgs e) {
            LoadingMessage.Visibility = Visibility.Visible;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
            MainSchedule.Visibility = Visibility.Collapsed;
            Missing2FACodeMessage.Visibility = Visibility.Collapsed;
            ScheduleLoadMessage.Visibility = Visibility.Collapsed;

            if (DataContext is ScheduleViewModel viewModel) {
                viewModel.NoUserLoggedInEvent += OnNoUserLoggedInEvent;
                viewModel.InvalidUserCredentialsEvent += OnUserInvalidCredentialsEvent;
                viewModel.Missing2FACodeEvent += OnMissing2FACodeEvent;
                viewModel.ScheduleLoadedEvent += OnScheduleLoaded;
                viewModel.WIPLoadStartedEvent += OnWIPLoadStartedEvent;
                viewModel.WIPLoadFinishedEvent += OnWIPLoadFinishedEvent;
                viewModel.IsViewLoaded = true;
            }

            Loaded -= ScheduleView_Loaded;
        }

        private void OnNoUserLoggedInEvent(object? sender, EventArgs e) {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Visible;
            MainSchedule.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
            Missing2FACodeMessage.Visibility = Visibility.Collapsed;

        }

        private void OnUserInvalidCredentialsEvent(object? sender, EventArgs e) {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            MainSchedule.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Visible;
            Missing2FACodeMessage.Visibility = Visibility.Collapsed;
        }

        private void OnMissing2FACodeEvent(object? sender, EventArgs e) {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            MainSchedule.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
            Missing2FACodeMessage.Visibility = Visibility.Visible;
        }

        private void OnScheduleLoaded(object? sender, EventArgs e) {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            MainSchedule.Visibility = Visibility.Visible;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
            Missing2FACodeMessage.Visibility = Visibility.Collapsed;
        }

        private void OnWIPLoadStartedEvent(object? sender, EventArgs e) {
            ScheduleLoadMessage.Visibility = Visibility.Visible;
        }

        private void OnWIPLoadFinishedEvent(object? sender, EventArgs e) {
            ScheduleLoadMessage.Visibility = Visibility.Collapsed;
        }

        private void ScheduleView_Unloaded(object sender, RoutedEventArgs e) {
            if (DataContext is ScheduleViewModel viewModel) {
                viewModel.IsViewLoaded = false;
            }
        }
        
        private Border CreateBorder(int row, int column) {
            Border border = new Border {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
            };

            // Set background color for the upper row and leftmost column
            if (row == 0) {
                border.Background = Brushes.CadetBlue;
            } else if (column == 0 && row != 0) {
                border.Background = Brushes.LightBlue;
            } else {
                border.Background = Brushes.White;
            }

            return border;
        }
    }
}
