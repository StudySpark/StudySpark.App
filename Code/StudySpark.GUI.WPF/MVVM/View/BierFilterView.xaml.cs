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

        private bool? _hertogJanChecked, _amstelChecked, _heinekenChecked, _grolschChecked = true;
        private bool? _kratChecked, _blikChecked, _flesChecked, _fustChecked, _trayChecked = true;
  
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
        private void CheckBox_Checked_Grolsch(object sender, RoutedEventArgs e)
        {
            _grolschChecked = CheckboxGrolsch.IsChecked;
            FireEvent();
        }

        private void CheckBox_Checked_Krat(object sender, RoutedEventArgs e)
        {
            _kratChecked = CheckboxKrat.IsChecked;
            FireEvent();
        }
        private void CheckBox_Checked_Blik(object sender, RoutedEventArgs e)
        {
            _blikChecked = CheckboxBlik.IsChecked;
            FireEvent();
        }
        private void CheckBox_Checked_Fles(object sender, RoutedEventArgs e)
        {
            _flesChecked = CheckboxFles.IsChecked;
            FireEvent();
        }
        private void CheckBox_Checked_Fust(object sender, RoutedEventArgs e)
        {
            _fustChecked = CheckboxFust.IsChecked;
            FireEvent();
        }

        private void CheckBox_Checked_Tray(object sender, RoutedEventArgs e)
        {
            _trayChecked = CheckboxTray.IsChecked;
            FireEvent();
        }
        private void FireEvent()
        {
            BierFilterEventArgs bierFilterEventArgs = new BierFilterEventArgs();

            bierFilterEventArgs.HertogJanChecked = _hertogJanChecked;
            bierFilterEventArgs.AmstelChecked = _amstelChecked;
            bierFilterEventArgs.HeinekenChecked = _heinekenChecked;
            bierFilterEventArgs.GrolschChecked = _grolschChecked;
            bierFilterEventArgs.KratIsChecked = _kratChecked;
            bierFilterEventArgs.BlikIsChecked = _blikChecked;
            bierFilterEventArgs.FlesIsChecked = _flesChecked;
            bierFilterEventArgs.FustIsChecked = _fustChecked;
            bierFilterEventArgs.TrayIsChecked = _trayChecked;

            ViewDataChangeEvent?.Invoke(this, bierFilterEventArgs);
        }
    }
}
