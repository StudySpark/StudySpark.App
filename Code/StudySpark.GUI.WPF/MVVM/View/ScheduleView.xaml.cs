using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudySpark.GUI.WPF.MVVM.View
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : UserControl
    {

        private Grid ScheduleGrid;

        public ScheduleView()
        {
            InitializeComponent();
            CreateGrid();
        }

        public void CreateGrid()
        {
            ScheduleGrid = new Grid();

            for (int row = 0; row < 6; row++)
            {
                ScheduleGrid.RowDefinitions.Add(new RowDefinition());
                for (int col = 0; col < 6; col++)
                {
                    ScheduleGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    // Create a Border control for each square
                    Border border = new Border
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                    };

                    // Add the Border control to the mainGrid
                    MainSchedule.Children.Add(border);

                    // Set the row and column for the Border control
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, col);
                }
            }
        }
    }
}
