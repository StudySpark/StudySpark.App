using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.Core
{
    public class BierFilterEventArgs : EventArgs
    {
        public bool? HertogJanChecked { get; set; } = true;
        public bool? AmstelChecked { get; set; } = true;
        public bool? HeinekenChecked { get; set; } = true;
        public bool? GrolschChecked { get; set; } = true;
        public bool? KratIsChecked { get; set; } = true;
        public bool? BlikIsChecked { get; set; } = true;
        public bool? FlesIsChecked { get; set; } = true;
        public bool? FustIsChecked { get; set; } = true;
        public bool? TrayIsChecked { get; set; } = true;
    }
}