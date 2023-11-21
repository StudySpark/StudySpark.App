using Microsoft.Win32;
using StudySpark.GUI.WPF.MVVM.Model;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudySpark.GUI.WPF.MVVM.View 
{
    /// <summary>
    /// Interaction logic for DiscoveryView.xaml
    /// </summary>
    public partial class FilesView : System.Windows.Controls.UserControl
    {
        public FilesView() {

            InitializeComponent();

        }

        private void AddMap_Click(object sender, RoutedEventArgs e)
        {

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult dr = dialog.ShowDialog();

            if (dr == DialogResult.OK)
            {
                FilesModel.AddFile();
            }

        }

        private void ReadDB()
        {
            
        }
    }
}
