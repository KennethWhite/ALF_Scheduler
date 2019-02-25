﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

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
            CreateCalendars(DateTime.Now.Year);
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
                if (x < 10) month = string.Format("0{0}", x);
                else month = x.ToString();

                calendar = new Calendar();
                calendar.DisplayDateStart = DateTime.Parse(string.Format("{0}/01/{1}", month, year));
                calendar.DisplayDateEnd = DateTime.Parse(string.Format("{0}/{1}/{2}", month, DateTime.DaysInMonth(year, x), year));
                calendar.IsTodayHighlighted = true;
                calendar.Margin = new Thickness(10);
                calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
                calendar.CalendarButtonStyle = (Style) Resources["CalendarButtonStyle"];

                HelperMethods.DateSelection(App.Facilities, calendar);

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
        private void previousYearButton_Click(object sender, RoutedEventArgs e) {
            calendarGrid.Children.RemoveRange(1, calendarGrid.Children.Count - 1);
            CreateCalendars(--curYear);
            EnableButton(nextYearButton);
            if (curYear == DateTime.Now.Year - 1) {
                DisableButton(previousYearButton);
            }
        }

        private void nextYearButton_Click(object sender, RoutedEventArgs e) {
            calendarGrid.Children.RemoveRange(1, calendarGrid.Children.Count - 1);
            CreateCalendars(++curYear);
            EnableButton(previousYearButton);
            if (curYear == DateTime.Now.Year + 1) {
                DisableButton(nextYearButton);
            }
        }
        private void EnableButton(Button button) {
            Path path = (Path)button.Content;
            path.Stroke = System.Windows.Media.Brushes.Black;
            button.IsEnabled = true;
        }

        private void DisableButton(Button button) {
            Path path = (Path)button.Content;
            path.Stroke = System.Windows.Media.Brushes.LightGray;
            button.IsEnabled = false;
        }
    }
}