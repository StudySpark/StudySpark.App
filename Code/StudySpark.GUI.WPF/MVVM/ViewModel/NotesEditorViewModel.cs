using StudySpark.Core.Generic;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class NotesEditorViewModel : ObservableObject {

        public GenericNoteListItem currentEditingNote {  get; set; }
    }
}
