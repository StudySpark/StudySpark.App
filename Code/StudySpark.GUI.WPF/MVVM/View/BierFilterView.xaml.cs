using System;
using System.Windows;
using System.Windows.Controls;
using StudySpark.GUI.WPF.Core;

namespace StudySpark.GUI.WPF.MVVM.View
{
    /// <summary>
    /// Interaction logic for BierFilterView.xaml
    /// </summary>
    public partial class BierFilterView : UserControl
    {
        public delegate void FilterViewEventHandler(object sender, BierFilterEventArgs e);
        public static event FilterViewEventHandler? ViewDataChangeEvent;

        private bool? _hertogJanChecked = false;
        private bool? _amstelChecked = false;
        private bool? _heinekenChecked = false;

        public BierFilterView()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked_HertogJan(object sender, RoutedEventArgs e)
        {
            _hertogJanChecked = CheckboxHertogJan.IsChecked;
            FireEvent();
        }
        private void CheckBox_Checked_Amstel(object sender, RoutedEventArgs e)
        {
            _amstelChecked = CheckboxAmstel.IsChecked;
            FireEvent();
        }

        private void CheckBox_Checked_Heineken(object sender, RoutedEventArgs e)
        {
            _heinekenChecked = CheckboxHeineken.IsChecked;
            FireEvent();
        }

        
        private void FireEvent()
        {
            BierFilterEventArgs bierFilterEventArgs = new BierFilterEventArgs();

            bierFilterEventArgs.HertogJanChecked = _hertogJanChecked;
            bierFilterEventArgs.AmstelChecked = _amstelChecked;
            bierFilterEventArgs.HeinekenChecked = _heinekenChecked;


            ViewDataChangeEvent?.Invoke(this, bierFilterEventArgs);
        }
    }
}
