using LibGit2Sharp;
using StudySpark.Core.Generic;
using StudySpark.Core.Repositories;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class NotesViewModel {
        public ObservableCollection<GenericNoteListItem> NoteListViewElements { get; set; } = new ObservableCollection<GenericNoteListItem>();
        public bool IsNoteListViewEmpty => NoteListViewElements.Count == 0;

        public RelayCommand NoteViewClick { get; private set; }
        public RelayCommand NoteEditClick { get; private set; }
        public RelayCommand NotePreDeleteClick { get; private set; }
        public RelayCommand NoteCreateCommand { get; private set; }

        public NotesViewModel() {
            NoteListViewElements = NotesRepository.Instance.NoteListViewElements;

            NoteViewClick = new RelayCommand((o) => {
                GenericNoteListItem? note = o as GenericNoteListItem;
                Debug.WriteLine($"NoteViewClick: {note?.NoteName}");
            });

            NoteEditClick = new RelayCommand((o) => {
                GenericNoteListItem? note = o as GenericNoteListItem;
                NotesEditorViewModel.CurrentEditingNote = note;
                MainViewManager.CurrentMainView = MainViewManager.NotesEditorVM;
            });

            NotePreDeleteClick = new RelayCommand((o) => {
                GenericNoteListItem? note = o as GenericNoteListItem;
                foreach (GenericNoteListItem item in NoteListViewElements) {
                    if (item.NoteName == note?.NoteName) {
                        NoteListViewElements.Remove(item);
                        break;
                    }
                }
                Debug.WriteLine($"NotePreDeleteClick: {note?.NoteName}");
            });

            NoteCreateCommand = new RelayCommand((o) => {
                string noteName = (o as TextBox).Text;
                string currentDate = DateTime.Now.ToString("dd-MM-yyyy");

                if (noteName.Length == 0) {
                    MessageBox.Show("Een naam is verplicht!");
                    return;
                }

                foreach (GenericNoteListItem existingNote in NotesRepository.Instance.NoteListViewElements) {
                    if (existingNote.NoteName.Equals(noteName)) {
                        MessageBox.Show("Een notitie met de opgegeven naam bestaat al!");
                        return;
                    }
                }

                GenericNoteListItem newNote = new GenericNoteListItem { NoteName = noteName, NoteDate = currentDate };
                NotesRepository.Instance.NoteListViewElements.Add(newNote);
                NotesEditorViewModel.CurrentEditingNote = newNote;
                MainViewManager.CurrentMainView = MainViewManager.NotesEditorVM;
            });


            //NoteListViewElements.Clear();
        }
    }
}
