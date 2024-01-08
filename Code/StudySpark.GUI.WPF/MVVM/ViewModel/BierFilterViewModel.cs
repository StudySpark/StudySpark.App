using StudySpark.GUI.WPF.Core;
using StudySpark.GUI.WPF.MVVM.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StudySpark.GUI.WPF.MVVM.ViewModel
{
    class BierFilterViewModel
    {
        public static bool? hertogIsChecked { get; set; } = true;
        public static bool? amstelIsChecked { get; set; } = true;
        public static bool? heinekenIsChecked { get; set; } = true;
        public static bool? grolschIsChecked { get; set; } = true;
        public static bool? kratIsChecked { get; set; } = true;
        public static bool? blikIsChecked { get; set; } = true;
        public static bool? flesIsChecked { get; set; } = true;
        public static bool? fustIsChecked { get; set; } = true;
        public static bool? trayIsChecked { get; set; } = true;


        public BierFilterViewModel()
        {
            BierFilterView.ViewDataChangeEvent += GetFilters;
        }

        private void GetFilters(object? sender, BierFilterEventArgs e)
        {
            hertogIsChecked = e.HertogJanChecked;
            amstelIsChecked = e.AmstelChecked;
            heinekenIsChecked = e.HeinekenChecked;
            grolschIsChecked = e.GrolschChecked;
            kratIsChecked = e.KratIsChecked;
            blikIsChecked = e.BlikIsChecked;
            flesIsChecked = e.FlesIsChecked;
            fustIsChecked = e.FustIsChecked;
            trayIsChecked = e.TrayIsChecked;
        }
    }
}
