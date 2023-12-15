using StudySpark.Core.Generic;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class NotesEditorViewModel : ObservableObject {

        public static GenericNoteListItem? CurrentEditingNote { get; set; }
        public double TextSize { get; set; } = 14;

        public RelayCommand EditorBackCommand { get; private set; }
        public RelayCommand EditorSaveCommand { get; private set; }

        public RelayCommand EditorImageCommand { get; private set; }
        public RelayCommand EditorColorCommand { get; private set; }
        public RelayCommand EditorFontCommand { get; private set; }
        public RelayCommand EditorTextSizeMinCommand { get; private set; }
        public RelayCommand EditorTextSizePlusCommand { get; private set; }
        public RelayCommand EditorAlignLeftCommand { get; private set; }
        public RelayCommand EditorAlignCenterCommand { get; private set; }
        public RelayCommand EditorAlignRightCommand { get; private set; }

        public NotesEditorViewModel() {

            //var textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            //textRange.Load(stream, DataFormats.Rtf);

            TextSize = GetDefaultFontSize();
            OnPropertyChanged(nameof(TextSize));

            EditorBackCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                MainViewManager.CurrentMainView = MainViewManager.NotesVM;
            });

            EditorSaveCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
                // textRange.Save(stream, DataFormats.Rtf);
                Debug.WriteLine(textRange.Text);
                MessageBox.Show("Opslaan is niet mogelijk.");
            });

            EditorImageCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("Image");
            });

            EditorColorCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("Color");
            });

            EditorFontCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("Font");
            });

            EditorTextSizeMinCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                double currentTextSize = TextSize;
                TextSize--;
                TextSize = TextSize < 1 ? 1 : TextSize;

                if (!ChangeLineFontSize(rtfEditor, TextSize)) {
                    TextSize = currentTextSize;
                }

                OnPropertyChanged(nameof(TextSize));
            });

            EditorTextSizePlusCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                double currentTextSize = TextSize;
                TextSize++;
                TextSize = TextSize > 96 ? 96 : TextSize;

                if (!ChangeLineFontSize(rtfEditor, TextSize)) {
                    TextSize = currentTextSize;
                }

                OnPropertyChanged(nameof(TextSize));
            });

            EditorAlignLeftCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ALeft");
            });

            EditorAlignCenterCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ACenter");
            });

            EditorAlignRightCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ARight");
            });
        }

        private bool ChangeLineFontSize(RichTextBox richTextBox, double newSize) {
            if (richTextBox.Selection.IsEmpty) {
                // If there is no selected text, you might want to handle this case accordingly
                return false;
            }

            // Apply the new font size to the selected text
            richTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
            return true;
        }

        private double GetDefaultFontSize() {
            // Create a temporary FlowDocument to get the default font size
            FlowDocument tempFlowDocument = new FlowDocument();

            // Assign the default font size to a temporary TextBlock
            TextBlock tempTextBlock = new TextBlock();
            tempFlowDocument.Blocks.Add(new Paragraph(new Run("TempText") { FontSize = tempTextBlock.FontSize }));

            // Use the FontSize of the temporary TextBlock as the default font size
            double defaultFontSize = tempTextBlock.FontSize;

            return defaultFontSize;
        }
    }
}
