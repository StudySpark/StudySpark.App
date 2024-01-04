using StudySpark.GUI.WPF.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudySpark.GUI.WPF.MVVM.View {
    /// <summary>
    /// Interaction logic for NotesEditorView.xaml
    /// </summary>
    public partial class NotesEditorView : UserControl {
       
        public NotesEditorView() {
            InitializeComponent();

            NotesEditorViewModel.LoadContentEvent += HandleLoadContentEvent;
        }

        private void HandleLoadContentEvent(object sender, LoadContentEventArgs e)
        {
            // Handle the loaded content here
            string loadedContent = e.Content;

            // Example: Display the content in the RichTextBox
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(loadedContent)))
            {
                TextRange text = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
                text.Load(stream, DataFormats.Rtf);
            }
        }
    }
}
