using StudySpark.Core;
using StudySpark.Core.Generic;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StudySpark.GUI.WPF.MVVM.ViewModel {
    public class NotesEditorViewModel : ObservableObject {

        private const int MIN_TEXT_SIZE = 6;
        private const int MAX_TEXT_SIZE = 96;
        private const string HIGHLIGHT_TRANSPARENT_HEX = "#FF2B2A3C";

        public static event EventHandler<LoadContentEventArgs> LoadContentEvent;

        public bool IsColorSelectorHighlightVisible { get; set; } = false;
        public bool IsColorSelectorTextVisible { get; set; } = false;
        public bool IsFontSelectorVisible { get; set; } = false;

        public ObservableCollection<GenericFontElement> FontElements { get; set; } = new ObservableCollection<GenericFontElement>();

        public static GenericNoteListItem? CurrentEditingNote { get; set; }
        public double TextSize { get; set; } = 12;

        public RelayCommand EditorBackCommand { get; private set; }
        public RelayCommand EditorSaveCommand { get; private set; }

        public RelayCommand ToggleEditorHightlightColorSelectorCommand { get; private set; }
        public RelayCommand EditorColorCommand { get; private set; }
        public RelayCommand EditorSetTextColorCommand { get; private set; }
        public RelayCommand EditorHightlightCommand { get; private set; }
        public RelayCommand EditorFontCommand { get; private set; }
        public RelayCommand EditorSetFontCommand { get; private set; }
        public RelayCommand EditorTextSizeMinCommand { get; private set; }
        public RelayCommand EditorTextSizePlusCommand { get; private set; }
        public RelayCommand EditorBulletListCommand { get; private set; }
        public RelayCommand EditorImageCommand { get; private set; }
        public RelayCommand EditorAlignLeftCommand { get; private set; }
        public RelayCommand EditorAlignCenterCommand { get; private set; }
        public RelayCommand EditorAlignRightCommand { get; private set; }

        public RelayCommand rtfEditor_SelectionChangedCommand { get; private set; }

        public NotesEditorViewModel() {

            if (CurrentEditingNote != null && CurrentEditingNote.Content != null) {
                LoadContentEvent?.Invoke(this, new LoadContentEventArgs(CurrentEditingNote.Content));
            }

            //var textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            //textRange.Load(stream, DataFormats.Rtf);

            FontElements = FontManager.GetAvailableFonts();

            TextSize = GetDefaultFontSize();
            OnPropertyChanged(nameof(TextSize));

            EditorBackCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                MainViewManager.CurrentMainView = MainViewManager.NotesVM;
            });

            EditorSaveCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                // Get the RTF text from the RichTextBox
                string rtfText;
                TextRange tr = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
                using (MemoryStream ms = new MemoryStream()) {
                    tr.Save(ms, DataFormats.Rtf);
                    rtfText = Encoding.UTF8.GetString(ms.ToArray());
                }
                CurrentEditingNote.Content = rtfText;
                // Create a new GenericNoteListItem and set the content
                MessageBox.Show("Opgeslagen.");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorColorCommand = new RelayCommand((o) => {
                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = !IsColorSelectorTextVisible;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorSetTextColorCommand = new RelayCommand((o) => {
                object[] parameters = o as object[];

                if (parameters != null && parameters.Length == 2) {
                    RichTextBox rtfEditor = parameters[0] as RichTextBox;
                    Brush buttonBackground = parameters[1] as Brush;

                    SolidColorBrush selectedBrush = new SolidColorBrush((buttonBackground as SolidColorBrush).Color);
                    ChangeSelectedTextColor(rtfEditor, selectedBrush);
                }

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            ToggleEditorHightlightColorSelectorCommand = new RelayCommand((o) => {
                IsColorSelectorHighlightVisible = !IsColorSelectorHighlightVisible;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorHightlightCommand = new RelayCommand((o) => {
                object[] parameters = o as object[];

                if (parameters != null && parameters.Length == 2) {
                    RichTextBox rtfEditor = parameters[0] as RichTextBox;
                    Brush buttonBackground = parameters[1] as Brush;

                    SolidColorBrush selectedBrush = new SolidColorBrush((buttonBackground as SolidColorBrush).Color);
                    HighlightSelectedTextRichTextBox(rtfEditor, selectedBrush);
                }

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorFontCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                //ChangeFontFamily(rtfEditor, "");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = !IsFontSelectorVisible;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorSetFontCommand = new RelayCommand((o) => {
                object[] parameters = o as object[];

                if (parameters != null && parameters.Length == 2) {
                    RichTextBox rtfEditor = parameters[0] as RichTextBox;
                    FontFamily fontFamily = parameters[1] as FontFamily;

                    ChangeFontFamily(rtfEditor, fontFamily);
                }

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorTextSizeMinCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                double currentTextSize = TextSize;
                TextSize--;
                TextSize = TextSize < MIN_TEXT_SIZE ? MIN_TEXT_SIZE : TextSize;

                if (!ChangeLineFontSize(rtfEditor, TextSize)) {
                    TextSize = currentTextSize;
                }

                OnPropertyChanged(nameof(TextSize));

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorTextSizePlusCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                double currentTextSize = TextSize;
                TextSize++;
                TextSize = TextSize > MAX_TEXT_SIZE ? MAX_TEXT_SIZE : TextSize;

                if (!ChangeLineFontSize(rtfEditor, TextSize)) {
                    TextSize = currentTextSize;
                }

                OnPropertyChanged(nameof(TextSize));

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorBulletListCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                InsertBulletList(rtfEditor);

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorImageCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog {
                    Filter = "Image Files |*.jpg; *.jpeg; *.png; *.gif; *.bmp|All files (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    string imagePath = openFileDialog.FileName;
                    AddImageToRichTextBox(rtfEditor, imagePath);
                }

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorAlignLeftCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ALeft");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorAlignCenterCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ACenter");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            EditorAlignRightCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ARight");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
                IsFontSelectorVisible = false;
                OnPropertyChanged(nameof(IsFontSelectorVisible));
            });

            rtfEditor_SelectionChangedCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                TextPointer textPointer = rtfEditor.CaretPosition;
                if (textPointer != null) {
                    Inline inline = textPointer.Parent as Inline;

                    if (inline != null) {
                        TextSize = (double)inline.GetValue(TextElement.FontSizeProperty);
                        OnPropertyChanged(nameof(TextSize));
                    }
                }
            });
        }

        private bool ChangeLineFontSize(RichTextBox richTextBox, double newSize) {
            if (richTextBox.Selection.IsEmpty) {
                return false;
            }
            richTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, newSize);
            return true;
        }

        private double GetDefaultFontSize() {
            FlowDocument tempFlowDocument = new FlowDocument();
            TextBlock tempTextBlock = new TextBlock();
            tempFlowDocument.Blocks.Add(new Paragraph(new Run("TempText") { FontSize = tempTextBlock.FontSize }));
            double defaultFontSize = tempTextBlock.FontSize;
            return defaultFontSize;
        }

        private bool ChangeSelectedTextColor(RichTextBox richTextBox, SolidColorBrush color) {
            if (richTextBox.Selection.IsEmpty) {
                return false;
            }
            TextRange selectedText = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);
            selectedText.ApplyPropertyValue(TextElement.ForegroundProperty, color);
            return true;
        }

        private bool HighlightSelectedTextRichTextBox(RichTextBox richTextBox, SolidColorBrush color) {
            if (richTextBox.Selection.IsEmpty) {
                return false;
            }
            if (color.Color.ToString() == HIGHLIGHT_TRANSPARENT_HEX) {
                color = Brushes.Transparent;
            }
            TextRange selectedText = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);
            selectedText.ApplyPropertyValue(TextElement.BackgroundProperty, color);
            return true;
        }

        private void AddImageToRichTextBox(RichTextBox richTextBox, string imagePath) {
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

            double aspectRatio = bitmapImage.Height / bitmapImage.Width;

            Image image = new Image {
                Source = bitmapImage,
                Width = Math.Min(400, bitmapImage.Width),
                Height = Math.Min(400, bitmapImage.Width) * aspectRatio
            };

            InlineUIContainer container = new InlineUIContainer(image);

            TextPointer insertionPosition = richTextBox.CaretPosition;

            if (insertionPosition != null) {
                var paragraph = new Paragraph(container);

                richTextBox.Document.Blocks.InsertBefore(insertionPosition.Paragraph, paragraph);
            }
        }

        private void InsertBulletList(RichTextBox richTextBox) {
            TextPointer insertionPosition = richTextBox.CaretPosition;

            if (insertionPosition != null) {
                var newParagraph = new Paragraph(new Run("• "));

                richTextBox.Document.Blocks.InsertAfter(insertionPosition.Paragraph, newParagraph);
            }
        }

        private void ChangeFontFamily(RichTextBox richTextBox, FontFamily font) {
            TextRange selection = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);

            if (selection != null) {
                if (font != null) {
                    selection.ApplyPropertyValue(TextElement.FontFamilyProperty, font);
                }
            }
        }

        //public void rtfEditor_SelectionChanged(object sender, RoutedEventArgs e) {
        //    if (sender is RichTextBox richTextBox) {
        //        TextPointer textPointer = richTextBox.Selection.Start;

        //        if (textPointer != null) {
        //            Paragraph paragraph = textPointer.Paragraph;
        //            TextSize = paragraph.FontSize;
        //            OnPropertyChanged(nameof(TextSize));
        //        }
        //    }
        //}
    }

    public class LoadContentEventArgs : EventArgs {
        public string Content { get; }

        public LoadContentEventArgs(string content) {
            Content = content;
        }
    }
}
