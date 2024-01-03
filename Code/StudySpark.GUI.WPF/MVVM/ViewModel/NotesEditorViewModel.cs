using StudySpark.Core.Generic;
using StudySpark.GUI.WPF.Core;
using System;
using System.Collections.Generic;
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

        public bool IsColorSelectorHighlightVisible { get; set; } = false;
        public bool IsColorSelectorTextVisible { get; set; } = false;

        public static GenericNoteListItem? CurrentEditingNote { get; set; }
        public double TextSize { get; set; } = 12;

        public RelayCommand EditorBackCommand { get; private set; }
        public RelayCommand EditorSaveCommand { get; private set; }

        public RelayCommand ToggleEditorHightlightColorSelectorCommand { get; private set; }
        public RelayCommand EditorColorCommand { get; private set; }
        public RelayCommand EditorSetTextColorCommand { get; private set; }
        public RelayCommand EditorHightlightCommand { get; private set; }
        public RelayCommand EditorFontCommand { get; private set; }
        public RelayCommand EditorTextSizeMinCommand { get; private set; }
        public RelayCommand EditorTextSizePlusCommand { get; private set; }
        public RelayCommand EditorBulletListCommand { get; private set; }
        public RelayCommand EditorImageCommand { get; private set; }
        public RelayCommand EditorAlignLeftCommand { get; private set; }
        public RelayCommand EditorAlignCenterCommand { get; private set; }
        public RelayCommand EditorAlignRightCommand { get; private set; }

        public RelayCommand rtfEditor_SelectionChangedCommand { get; private set; }

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

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
            });

            EditorColorCommand = new RelayCommand((o) => {
                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = !IsColorSelectorTextVisible;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
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
            });

            ToggleEditorHightlightColorSelectorCommand = new RelayCommand((o) => {
                IsColorSelectorHighlightVisible = !IsColorSelectorHighlightVisible;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
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
            });

            EditorFontCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("Font");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
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
            });

            EditorBulletListCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                InsertBulletList(rtfEditor);

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
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
            });

            EditorAlignLeftCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ALeft");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
            });

            EditorAlignCenterCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ACenter");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
            });

            EditorAlignRightCommand = new RelayCommand((o) => {
                RichTextBox rtfEditor = o as RichTextBox;

                Debug.WriteLine("ARight");

                IsColorSelectorHighlightVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorHighlightVisible));
                IsColorSelectorTextVisible = false;
                OnPropertyChanged(nameof(IsColorSelectorTextVisible));
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
}
