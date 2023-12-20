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
            bool HertogJanChecked = (bool) filters[0];
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
                FilteredList.Add("Hertog Jan");
            }
            if (AmstelChecked)
            {
                  FilteredList.Add("Amstel");
            }
            if (HeinekenChecked)
            {
                FilteredList.Add("Heineken");
            }
            if (GrolschChecked)
            {
                 FilteredList.Add("Grolsch");
            }


            if (KratChecked)
            {
                 FilteredList.Add("Krat van 24 flesjes á 0,30 liter");
                 FilteredList.Add("Krat van 18 flesjes á 0,50 liter");
            }

            if (FlesChecked)
            {
                FilteredList.Add("Flesje á 0,25 liter");
                FilteredList.Add("Fles á 0,30 liter");
                FilteredList.Add("Fles á 0,50 liter");
                FilteredList.Add("Set van 6 flesjes á 0,25 liter");
                FilteredList.Add("Set van 6 flesjes á 0,30 liter");
                FilteredList.Add("Set van 8 flesjes á 0,25 liter");               
                FilteredList.Add("Set van 10 flesjes á 0,25 liter");               
                FilteredList.Add("Doos van 12 flesjes 0,25 liter");                
                FilteredList.Add("Doos met 20 flesjes van 0,25 liter");
                FilteredList.Add("Fles á 1,50 liter");
                FilteredList.Add("Set van 4 flesjes á 0,45 liter");
                FilteredList.Add("Fles á 0,45 liter");
            }
            

            if (BlikChecked)
            {
                FilteredList.Add("Blik van 0,50 liter");              
                FilteredList.Add("Blikje van 0,33 liter");               
                FilteredList.Add("Set van 6 blikjes 0,33 liter");               
                FilteredList.Add("Set van 6 blikken á 0,50 liter");
                FilteredList.Add("Set van 8 blikken á 0,50 liter");
                FilteredList.Add("Set van 12 blikjes á 0,33 liter");           
        }

            if (FustChecked)
            {
                FilteredList.Add("Fust van 50 liter");
                FilteredList.Add("Torp van 2 liter");
                FilteredList.Add("Fust van 20 liter");
                FilteredList.Add("Perfect Draft fust");
                FilteredList.Add("Tapvat á 5 liter");
                FilteredList.Add("Blade fust van 8 liter");
                FilteredList.Add("Fust van 30 liter");
            }

            if (TrayChecked)
            {
                FilteredList.Add("Tray met 24 blikjes 0,33 liter");
            }
          
            return FilteredList;
        }
    }
}
