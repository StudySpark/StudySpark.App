using StudySpark.Core;
using System;
using System.Windows;

namespace StudySpark.GUI.WPF {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                Exception ex = (Exception)args.ExceptionObject;
                Logger.Error(ex.StackTrace);
            };

            base.OnStartup(e);
        }
    }
}
