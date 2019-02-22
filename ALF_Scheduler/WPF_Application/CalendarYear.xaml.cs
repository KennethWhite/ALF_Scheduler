using System;
using System.Windows;
using System.Windows.Controls;

namespace ALF_Scheduler
{
    /// <summary>
    ///     Interaction logic for CalendarYear.xaml
    /// </summary>
    public partial class CalendarYear : Page
    {
        /// <summary>
        ///     The CalendarYear constructor intializes the 12 month calendar in the current year,
        ///     using the helper method <see cref="CreateCalendars" />.
        /// </summary>
        public CalendarYear()
        {
            InitializeComponent();
            calendarYearLabel.Content += DateTime.Now.Year.ToString();
            CreateCalendars();
        }

        /// <summary>
        ///     This helper method creates 12 individual calendar objects in the current year
        ///     with blackout dates for dates passed, and selected future dates.
        /// </summary>
        public void CreateCalendars()
        {
            Calendar calendar;
            string month;
            var year = DateTime.Now.Year;

            var row = 1;
            var column = 0;
            for (var x = 1; x <= 12; x++)
            {
                if (x < 10) month = string.Format("0{0}", x);
                else month = x.ToString();

                calendar = new Calendar();
                calendar.DisplayDateStart = DateTime.Parse(string.Format("{0}/01/{1}", month, year));
                calendar.DisplayDateEnd =
                    DateTime.Parse(string.Format("{0}/{1}/{2}", month, DateTime.DaysInMonth(year, x), year));
                calendar.IsTodayHighlighted = true;
                calendar.Margin = new Thickness(10);
                calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
                calendar.CalendarButtonStyle = (Style) Resources["CalendarButtonStyle"];

                //TODO add blackout and selected dates
                Grid.SetRow(calendar, row);
                Grid.SetColumn(calendar, column);
                calendarGrid.Children.Add(calendar);

                if (x == 4 || x == 8)
                {
                    row++;
                    column = 0;
                }
                else
                {
                    column++;
                }
            }
        }
    }
}