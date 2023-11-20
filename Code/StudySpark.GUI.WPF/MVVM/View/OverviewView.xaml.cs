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
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class OverviewView : UserControl {
        public OverviewView() {
            InitializeComponent();

            UserGreetingText.Text = GetGreeting(DateTime.Now.Hour, Environment.UserName);
        }

        private string GetGreeting(int hour, string name) {
            string greeting;

            if (hour >= 5 && hour < 12) {
                greeting = $"Good morning {name}!";
            } else if (hour >= 12 && hour < 17) {
                greeting = $"Good afternoon {name}!";
            } else if (hour >= 17 && hour < 21) {
                greeting = $"Good evening {name}!";
            } else {
                greeting = $"Good night {name}!";
            }

            return greeting;
        }
    }
}
