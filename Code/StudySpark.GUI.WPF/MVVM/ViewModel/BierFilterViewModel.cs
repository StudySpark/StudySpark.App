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
        public static bool? hertogIsChecked, amstelIsChecked, heinekenIsChecked = true;
        public BierFilterViewModel()
        {
            BierFilterView.ViewDataChangeEvent += GetFilters;
        }

        private void GetFilters(object? sender, BierFilterEventArgs e)
        {
            hertogIsChecked = e.HertogJanChecked;
            amstelIsChecked = e.AmstelChecked;
            heinekenIsChecked = e.HeinekenChecked;
        }
    }
}
