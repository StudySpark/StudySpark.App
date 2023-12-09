using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.Core
{
    public class BierFilterEventArgs : EventArgs
    {
        public bool? HertogJanChecked
        {
            get; set;
        } = true;
        public bool? AmstelChecked
        {
            get; set;
        } = true;
        public bool? HeinekenChecked
        {
            get; set;
        } = true;
    }
}
