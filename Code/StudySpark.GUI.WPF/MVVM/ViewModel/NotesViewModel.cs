using StudySpark.Core.Generic;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class NotesViewModel {

        public ObservableCollection<GenericNoteListItem> NoteListViewElements { get; set; } = new ObservableCollection<GenericNoteListItem>();

        public RelayCommand NoteViewClick { get; private set; }
        public RelayCommand NoteEditClick { get; private set; }
        public RelayCommand NotePreDeleteClick { get; private set; }

        public NotesViewModel() {
            NoteViewClick = new RelayCommand((o) => {
                Debug.WriteLine($"NoteViewClick: {(o as GenericNoteListItem)?.NoteName}");
            });

            NoteEditClick = new RelayCommand((o) => {
                Debug.WriteLine($"NoteEditClick: {(o as GenericNoteListItem)?.NoteName}");
            });

            NotePreDeleteClick = new RelayCommand((o) => {
                Debug.WriteLine($"NotePreDeleteClick: {(o as GenericNoteListItem)?.NoteName}");
            });


            NoteListViewElements.Clear();

            NoteListViewElements = new ObservableCollection<GenericNoteListItem>
            {
                new GenericNoteListItem { NoteName = "Dummy Note 1", NoteDate = "2023-01-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 2", NoteDate = "2023-02-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 3", NoteDate = "2023-03-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 4", NoteDate = "2023-04-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 5", NoteDate = "2023-05-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 6", NoteDate = "2023-06-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 7 With a long name", NoteDate = "2023-07-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 8", NoteDate = "2023-08-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 9", NoteDate = "2023-09-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 10", NoteDate = "2023-01-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 11", NoteDate = "2023-01-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 12", NoteDate = "2023-02-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 13", NoteDate = "2023-03-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 14", NoteDate = "2023-04-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 15", NoteDate = "2023-05-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 16", NoteDate = "2023-06-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 17", NoteDate = "2023-07-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 18", NoteDate = "2023-08-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 19", NoteDate = "2023-09-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 20", NoteDate = "2023-01-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 21", NoteDate = "2023-01-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 22", NoteDate = "2023-02-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 23", NoteDate = "2023-03-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 24", NoteDate = "2023-04-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 25", NoteDate = "2023-05-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 26", NoteDate = "2023-06-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 27", NoteDate = "2023-07-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 28", NoteDate = "2023-08-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 29", NoteDate = "2023-09-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 30", NoteDate = "2023-01-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 31", NoteDate = "2023-01-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 32", NoteDate = "2023-02-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 33", NoteDate = "2023-03-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 34", NoteDate = "2023-04-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 35", NoteDate = "2023-05-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 36", NoteDate = "2023-06-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 37", NoteDate = "2023-07-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 38", NoteDate = "2023-08-01" },
                new GenericNoteListItem { NoteName = "Dummy Note 39", NoteDate = "2023-09-01" },
            };
        }
    }
}
