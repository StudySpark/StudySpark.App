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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;

namespace StudySpark.GUI.WPF.MVVM.View {
    /// <summary>
    /// Interaction logic for NotesEditorView.xaml
    /// </summary>
    public partial class NotesEditorView : UserControl {

        private static string content = "";

        public NotesEditorView() {
            InitializeComponent();

            NotesEditorViewModel.LoadContentEvent += HandleLoadContentEvent;

            Loaded += windowLoaded;
        }

        private void windowLoaded(object sender, RoutedEventArgs e) {
            loadRTF();
        }

        private void HandleLoadContentEvent(object sender, LoadContentEventArgs e) {
            content = e.Content;
        }

        void loadRTF() {
            if (content.Length == 0) {
                return;
            }

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content))) {
                stream.Seek(0, SeekOrigin.Begin);
                rtfEditor.SelectAll();
                rtfEditor.Selection.Load(stream, DataFormats.Rtf);
            }

            content = "";
        }

    }
}
