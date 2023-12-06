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

        public GradesView() {
            InitializeComponent();

            GradesViewModel.NoUserLoggedInEvent += OnNoUserLoggedInEvent;

            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            GradesItemControl.Visibility = Visibility.Visible;
        }

        private void OnNoUserLoggedInEvent(object? sender, EventArgs e) {
            NotLoggedInMessage.Visibility = Visibility.Visible;
            GradesItemControl.Visibility = Visibility.Collapsed;
        }
    }
}
