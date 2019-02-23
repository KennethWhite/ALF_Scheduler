using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ALF_Scheduler.Models;

namespace ALF_Scheduler
{
    internal class HelperMethods
    {
        /// <summary>
        ///     This is a helper method for navigating to the calendarYear page.
        /// </summary>
        public static void OpenCalendar(Page page)
        {
            page.NavigationService.Navigate(page);
        }

        /// <summary>
        ///     This method adds each facility's proposed date (from the list of facilities given) into the calendar object.
        /// </summary>
        /// <param name="facilities">The list of facilities.</param>
        /// <param name="month">The Calendar object the dates should be added to.</param>
        public static void DateSelection(List<Facility> facilities, Calendar month)
        {
            foreach (var facility in facilities)
            {
                AddSelectedDates(facility, month);
                AddBlackoutDates(facility, month);
            }
        }

        /// <summary>
        ///     This is a helper method that highlights a facility's proposed inspection date in the calendar object.
        /// </summary>
        /// <param name="facility">The facility to pull the date from.</param>
        /// <param name="month">The Calendar object the date should be added to.</param>
        public static void AddSelectedDates(Facility facility, Calendar month)
        {
            month.SelectedDates.Add(DateTime.Parse(facility.ProposedDate.ToString()));
        }

        /// <summary>
        ///     This is a helper method that 'X's through a facility's most recent inspection date in the calendar object.
        /// </summary>
        /// <param name="facility">The facility to pull the date from.</param>
        /// <param name="month">The Calendar object the date should be added to.</param>
        public static void AddBlackoutDates(Facility facility, Calendar month)
        {
            month.BlackoutDates.Add(
                new CalendarDateRange(DateTime.Parse(facility.MostRecentFullInspection.ToString())));
        }
    }
}