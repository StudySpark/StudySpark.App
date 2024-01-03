using StudySpark.GUI.WPF.MVVM.ViewModel;
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

        public ScheduleView()
        {
            InitializeComponent();

            Loaded += GradesView_Loaded;
            Unloaded += GradesView_Unloaded;

            CreateGrid();
        }

        private void GradesView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingMessage.Visibility = Visibility.Visible;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;

            if (DataContext is ScheduleViewModel viewModel)
            {
                viewModel.NoUserLoggedInEvent += OnNoUserLoggedInEvent;
                viewModel.InvalidUserCredentialsEvent += OnUserInvalidCredentialsEvent;
                viewModel.IsViewLoaded = true;
            }

            Loaded -= GradesView_Loaded;
        }

        private void OnNoUserLoggedInEvent(object? sender, EventArgs e)
        {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Visible;
            InvalidCredentialsMessage.Visibility = Visibility.Collapsed;
        }

        private void OnUserInvalidCredentialsEvent(object? sender, EventArgs e)
        {
            LoadingMessage.Visibility = Visibility.Collapsed;
            NotLoggedInMessage.Visibility = Visibility.Collapsed;
            InvalidCredentialsMessage.Visibility = Visibility.Visible;
        }

        private void GradesView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is GradesViewModel viewModel)
            {
                viewModel.IsViewLoaded = false;
            }
        }

        public void CreateGrid()
        {

            for (int row = 0; row < 6; row++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                MainSchedule.RowDefinitions.Add(rowDefinition);

                ColumnDefinition columnDefinition = new ColumnDefinition();
                MainSchedule.ColumnDefinitions.Add(columnDefinition);

                for (int col = 0; col < 6; col++)
                {
                    // Create a Border control for each square
                    Border border = CreateBorder(row, col);

                    // Add the Border control to the mainGrid
                    MainSchedule.Children.Add(border);

                    // Set the row and column for the Border control
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, col);

                    if ((row == 3 && col == 5) || (row == 4 && col == 2))
                    {
                        // Create a Grid to hold both Rectangle and TextBlock
                        Grid cellGrid = new Grid();

                        // Create a Rectangle control
                        Rectangle rectangle = new Rectangle
                        {
                            Fill = Brushes.DarkGray, // Set the fill color as per your requirement
                        };

                        // Add the Rectangle control to the cell's Grid
                        cellGrid.Children.Add(rectangle);

                        // Set the row and column for the Rectangle control
                        Grid.SetRow(rectangle, 0);
                        Grid.SetColumn(rectangle, 0);

                        // Create a TextBlock for multiple lines of text
                        TextBlock textBlock = new TextBlock
                        {
                            Text = "[Vak] \n [Docent] \n [Lokaal]",
                            TextWrapping = TextWrapping.Wrap,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                        };

                        // Add the TextBlock control to the cell's Grid
                        cellGrid.Children.Add(textBlock);

                        // Set the row and column for the TextBlock control
                        Grid.SetRow(textBlock, 0);
                        Grid.SetColumn(textBlock, 0);

                        // Add the cell's Grid to the mainGrid
                        MainSchedule.Children.Add(cellGrid);

                        // Set the row and column for the cell's Grid
                        Grid.SetRow(cellGrid, row);
                        Grid.SetColumn(cellGrid, col);
                        Grid.SetRowSpan(cellGrid, 2);
                    }
                }
            }
        }

        private Border CreateBorder(int row, int column)
        {
            Border border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
            };

            // Set background color for the upper row and leftmost column
            if (row == 0)
            {
                border.Background = Brushes.CadetBlue;
            }
            else if (column == 0 && row != 0)
            {
                border.Background = Brushes.LightBlue;
            }
            else
            {
                border.Background = Brushes.White;
            }

            return border;
        }
    }
}
