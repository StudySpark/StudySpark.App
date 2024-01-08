using StudySpark.Core.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.Repositories {
    public class NotesRepository {
        public ObservableCollection<GenericNoteListItem> NoteListViewElements { get; set; } = new ObservableCollection<GenericNoteListItem>();

        private static NotesRepository? _instance;
        public static NotesRepository Instance { get { if (_instance == null) { _instance = new NotesRepository(); } return _instance; } }
    }
}
