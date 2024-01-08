using System.Windows;
using System.Windows.Controls;

namespace StudySpark.GUI.WPF.Core {
    public static class RichTextBoxBehavior {
        public static readonly DependencyProperty SelectionChangedCommandProperty =
            DependencyProperty.RegisterAttached(
                "SelectionChangedCommand",
                typeof(System.Windows.Input.ICommand),
                typeof(RichTextBoxBehavior),
                new PropertyMetadata(null, OnSelectionChangedCommandChanged));

        public static System.Windows.Input.ICommand GetSelectionChangedCommand(DependencyObject obj) {
            return (System.Windows.Input.ICommand)obj.GetValue(SelectionChangedCommandProperty);
        }

        public static void SetSelectionChangedCommand(DependencyObject obj, System.Windows.Input.ICommand value) {
            obj.SetValue(SelectionChangedCommandProperty, value);
        }

        private static void OnSelectionChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is RichTextBox richTextBox) {
                richTextBox.SelectionChanged -= RichTextBox_SelectionChanged;
                if (e.NewValue is System.Windows.Input.ICommand newCommand) {
                    richTextBox.SelectionChanged += RichTextBox_SelectionChanged;
                }
            }
        }

        public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(RichTextBoxBehavior),
            new PropertyMetadata(null));

        public static object GetCommandParameter(DependencyObject obj) {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value) {
            obj.SetValue(CommandParameterProperty, value);
        }

        private static void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e) {
            if (sender is RichTextBox richTextBox) {
                var command = GetSelectionChangedCommand(richTextBox);
                var commandParameter = GetCommandParameter(richTextBox);
                command?.Execute(commandParameter);
            }
        }
    }
}
