using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class ScheduleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand ScheduleLoginCommand { get; set; }

        public ScheduleViewModel() 
        {

            ScheduleLoginCommand = new RelayCommand(o => {
                MainViewManager.CurrentMainView = MainViewManager.LoginVM;
                //CurrentView = LoginVM;
            });

        }

    }
}
