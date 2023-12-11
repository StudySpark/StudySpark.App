using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    public class BierAanbiedingenViewModel : ObservableObject
    {
        public static event EventHandler? BierAanbiedingenClickedEvent;

        public AlleBierAanbiedingenViewModel AlleVM { get; set; }
        public BookmarkedAanbiedingenViewModel BookmarkedVM { get; set; }
        public BijnaVerlopenBierAanbiedingenViewModel BijnaVerlopenVM {  get; set; }    
        public VerlopenBierAanbiedingenViewModel VerlopenVM { get; set; }

        public RelayCommand AlleCommand { get ; set; }
        public RelayCommand BookmarkedCommand { get; set; }
        public RelayCommand BijnaVerlopenCommand { get; set; }        
        public RelayCommand VerlopenCommand { get; set; }

        private object currentTab;
        public object CurrentTab
        {
            get 
            {
                return currentTab; 
            } 
            set 
            { 
                currentTab = value;
                OnPropertyChanged();
            }
        }


        public BierAanbiedingenViewModel()
        {
            AlleVM = new AlleBierAanbiedingenViewModel();
            BookmarkedVM = new BookmarkedAanbiedingenViewModel();
            BijnaVerlopenVM = new BijnaVerlopenBierAanbiedingenViewModel();
            VerlopenVM = new VerlopenBierAanbiedingenViewModel();

            CurrentTab = AlleVM;
            

            AlleCommand = new RelayCommand(o =>
            {
                BierAanbiedingenClickedEvent?.Invoke(null, new EventArgs());
                CurrentTab = AlleVM;
            }); 
            BookmarkedCommand = new RelayCommand(o =>
            {
                CurrentTab = BookmarkedVM;
            });
            BijnaVerlopenCommand = new RelayCommand(o =>
            {
                CurrentTab = BijnaVerlopenVM;
            });
            VerlopenCommand = new RelayCommand(o =>
            {
                CurrentTab = VerlopenVM;
            });
        }
    }
}
