using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.GUI.WPF.Core
{
    class FilteredBeerList
    {
        public List<string> SetFilteredList(List<bool?> filters)
        {
            bool HertogJanChecked = (bool)filters[0];
            bool AmstelChecked = (bool)filters[1];
            bool HeinekenChecked = (bool)filters[2];
            bool GrolschChecked = (bool)filters[3];
            bool KratChecked = (bool)filters[4];
            bool BlikChecked = (bool)filters[5];
            bool FlesChecked = (bool)filters[6];
            bool FustChecked = (bool)filters[7];
            bool TrayChecked = (bool)filters[8];

            List<string> FilteredList = new();

            if (HertogJanChecked)
            {
                FilteredList.Add("hertog jan");
            }
            if (AmstelChecked)
            {
                FilteredList.Add("amstel");
            }
            if (HeinekenChecked)
            {
                FilteredList.Add("heineken");
            }
            if (GrolschChecked)
            {
                FilteredList.Add("grolsch");
            }


            if (KratChecked)
            {
                FilteredList.Add("krat");
            }
            if (FlesChecked)
            {
                FilteredList.Add("fles");
            }
            if (BlikChecked)
            {
                FilteredList.Add("blik");
            }
            if (FustChecked)
            {
                FilteredList.Add("fust");
                FilteredList.Add("torp");
                FilteredList.Add("tapvat");
            }
            if (TrayChecked)
            {
                FilteredList.Add("tray");
            }
            return FilteredList;
        }
    }
}
