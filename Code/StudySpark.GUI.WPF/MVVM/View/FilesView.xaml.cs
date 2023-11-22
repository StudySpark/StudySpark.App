using StudySpark.GUI.WPF.MVVM.Model;
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
    /// Interaction logic for DiscoveryView.xaml
    /// </summary>
    public partial class FilesView : UserControl {
        public FilesView() {
            InitializeComponent();
            SizeChanged += UserControl_SizeChanged;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update the folderPanel width when the size changes
            FilesControl.Width = e.NewSize.Width;
        }
    }
}
