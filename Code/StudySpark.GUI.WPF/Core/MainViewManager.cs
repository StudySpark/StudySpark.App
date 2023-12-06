using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.Core {
    public class MainViewManager {

        public static event EventHandler? CurrentMainViewEvent;


        private static object? _currentMainView;
        public static object? CurrentMainView {
            get { return _currentMainView; }
            set {
                _currentMainView = value;
                CurrentMainViewEvent?.Invoke(null, EventArgs.Empty);
            }
        }
    }
}
