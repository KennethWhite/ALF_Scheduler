using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using ALF_Scheduler.Models;
using System.Linq;

namespace WPF_Application
{
    /// <summary>
    ///     Interaction logic for CalendarYear.xaml
    /// </summary>
    public partial class CalendarYear : Page {
        private int curYear;

        /// <summary>
        ///     The CalendarYear constructor intializes the 12 month calendar in the current year,
        ///     using the helper method <see cref="CreateCalendars" />.
        /// </summary>
        public CalendarYear()
        {
            InitializeComponent();
            CreateCalendars(DateTime.Today.Year);
        }

        /// <summary>
        ///     This helper method creates 12 individual calendar objects in the current year
        ///     with blackout dates for dates passed, and selected future dates.
        /// </summary>
        public void CreateCalendars(int year)
        {
            calendarYearLabel.Content = String.Format("Calendar Year {0}", year.ToString());
            Calendar calendar;
            string month;
            curYear = year;

            var row = 1;
            var column = 0;
            for (var x = 1; x <= 12; x++)
            {
                month = x.ToString();

                calendar = new Calendar {
                    DisplayDateStart = DateTime.Parse(string.Format("{0}/01/{1}", month, year)),
                    DisplayDateEnd = DateTime.Parse(string.Format("{0}/{1}/{2}", month, DateTime.DaysInMonth(year, x), year)),
                    IsTodayHighlighted = true,
                    Margin = new Thickness(10),
                    SelectionMode = CalendarSelectionMode.MultipleRange,
                    IsHitTestVisible = true
                };
                calendar.GotMouseCapture += Calendar_GotMouseCapture;

                HelperMethods.DateSelection(calendar);

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

        /// <summary>
        ///     This method grabs the selected date from the calendar to be displayed in the details tab
        ///     if a facility's proposed date matches the selected date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calendar_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e) {
            var source = e.OriginalSource as CalendarDayButton;
            if (source != null) {
                DateTime date = DateTime.Parse(source.DataContext.ToString());
                Facility facility = App.Facilities.FirstOrDefault(x => x.ProposedDate.Date.Equals(date.Date));
                if (facility != null) {
                    NavigationService.Navigate(App.HomePage);
                    App.HomePage.DisplayFacility(facility);
                }
            }
        }

        /// <summary>
        ///     This method handles the left arrow being clicked. 
        ///     It updates the calendars to be the previous year.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousYearButton_Click(object sender, RoutedEventArgs e) {
            calendarGrid.Children.RemoveRange(1, calendarGrid.Children.Count - 1);
            CreateCalendars(--curYear);
            EnableButton(nextYearButton);
            if (curYear == DateTime.Today.Year - 1) {
                DisableButton(previousYearButton);
            }
        }

        /// <summary>
        ///     This method handles the right arrow being clicked.
        ///     It updates the calendars to be the next year.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextYearButton_Click(object sender, RoutedEventArgs e) {
            calendarGrid.Children.RemoveRange(1, calendarGrid.Children.Count - 1);
            CreateCalendars(++curYear);
            EnableButton(previousYearButton);
            if (curYear == DateTime.Today.Year + 1) {
                DisableButton(nextYearButton);
            }
        }

        /// <summary>
        ///     This enables the arrow button.
        /// </summary>
        /// <param name="button">The button to be enabled.</param>
        private void EnableButton(Button button) {
            Path path = (Path)button.Content;
            path.Stroke = System.Windows.Media.Brushes.Black;
            button.IsEnabled = true;
        }

        /// <summary>
        ///     This disables the arrow button.
        /// </summary>
        /// <param name="button">The button to be disabled.</param>
        private void DisableButton(Button button) {
            Path path = (Path)button.Content;
            path.Stroke = System.Windows.Media.Brushes.LightGray;
            button.IsEnabled = false;
        }
    }
}