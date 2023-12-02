using StudySpark.GUI.WPF.MVVM.ViewModel;
using System;
using System.Collections.Generic;
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

        private GradesViewModel ViewModel => (GradesViewModel)DataContext;

        public GradesView() {
            InitializeComponent();

            ViewModel.LoginButtonClicked += DismissAccountSettingsPopup;
            ViewModel.LogoutButtonClicked += DismissAccountSettingsPopup;
        }

        private void DismissAccountSettingsPopup(object sender, EventArgs e) {
            // Set IsChecked to false
            EducatorAccountSettingsButton.IsChecked = false;
        }
    }
}
