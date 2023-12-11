using StudySpark.GUI.WPF.MVVM.ViewModel;
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

        private static OverviewViewModel? _overviewVM;
        public static OverviewViewModel? OverviewVM { get { if (_overviewVM == null) { _overviewVM = new OverviewViewModel(); } return _overviewVM; } set { _overviewVM = value; } }


        private static NotesViewModel? _notesVM;
        public static NotesViewModel? NotesVM { get { if (_notesVM == null) { _notesVM = new NotesViewModel(); } return _notesVM; } set { _notesVM = value; } }


        private static FilesViewModel? _filesVM;
        public static FilesViewModel? FilesVM { get { if (_filesVM == null) { _filesVM = new FilesViewModel(); } return _filesVM; } set { _filesVM = value; } }


        private static ScheduleViewModel? _scheduleVM;
        public static ScheduleViewModel? ScheduleVM { get { if (_scheduleVM == null) { _scheduleVM = new ScheduleViewModel(); } return _scheduleVM; } set { _scheduleVM = value; } }


        private static GradesViewModel? _gradesVM;
        public static GradesViewModel? GradesVM { get { if (_gradesVM == null) { _gradesVM = new GradesViewModel(); } return _gradesVM; } set { _gradesVM = value; } }


        private static LoginViewModel? _loginVM;
        public static LoginViewModel? LoginVM { get { if (_loginVM == null) { _loginVM = new LoginViewModel(); } return _loginVM; } set { _loginVM = value; } }


        private static GitViewModel? _gitVM;
        public static GitViewModel? GitVM { get { if (_gitVM == null) { _gitVM = new GitViewModel(); } return _gitVM; } set { _gitVM = value; } }
        
        
        private static BierAanbiedingenViewModel? _bierVM;
        public static BierAanbiedingenViewModel? BierVM { get { if (_bierVM == null) { _bierVM = new BierAanbiedingenViewModel(); } return _bierVM; } set { _bierVM = value; } }

       
        private static TimelineViewModel? _timelineVM;
        public static TimelineViewModel? TimelineVM { get { if (_timelineVM == null) { _timelineVM = new TimelineViewModel(); } return _timelineVM; } set { _timelineVM = value; } }
    }
}
