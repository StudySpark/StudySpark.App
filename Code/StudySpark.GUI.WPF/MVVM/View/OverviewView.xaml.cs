using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

            Thread thread = new Thread(() => {
                string greetingText = GetGreeting(DateTime.Now.Hour, CapitalizeFirstLetter(Environment.UserName));
                int index = 0;
                while (index <= greetingText.Length) {
                    try {
                        Application.Current.Dispatcher.Invoke(() => {
                            UserGreetingText.Text = greetingText.Substring(0, index);
                        });
                    } catch (NullReferenceException) { }
                    index++;
                    Thread.Sleep(25);
                }
            });
            thread.Start();
        }

        private string GetGreeting(int hour, string name) {
            string greeting;

            if (hour >= 5 && hour < 12) {
                greeting = $"Goede morgen {name}!";
            } else if (hour >= 12 && hour < 17) {
                greeting = $"Goede middag {name}!";
            } else if (hour >= 17 && hour < 21) {
                greeting = $"Goede avond {name}!";
            } else {
                greeting = $"Goede nacht {name}!";
            }

            return greeting;
        }

        private string CapitalizeFirstLetter(string input) {
            if (string.IsNullOrEmpty(input)) {
                return input;
            }

            char[] charArray = input.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);

            return new string(charArray);
        }
    }
}
