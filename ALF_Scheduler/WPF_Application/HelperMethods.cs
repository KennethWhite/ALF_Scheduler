using ALF_Scheduler;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ALF_Scheduler.Models;

namespace WPF_Application
{
    internal class HelperMethods
    {
        /// <summary>
        ///     This method adds each facility's proposed date (from the list of facilities given) into the calendar object.
        /// </summary>
        /// <param name="facilities">The list of facilities.</param>
        /// <param name="month">The Calendar object the dates should be added to.</param>
        public static void DateSelection(Calendar month)
        {
            foreach (var facility in App.Facilities) {
                var date = new DateTime();
                var black = facility.MostRecentFullInspection.InspectionDate;
                if (month.DisplayDateStart != null) {
                    date = (DateTime)month.DisplayDateStart;
                    var proposed = facility.ProposedDate;
                    if (date.Month.Equals(proposed.Month) && date.Year.Equals(proposed.Year)) {
                        AddSelectedDates(facility, month);
                    } else if (date.Month.Equals(black.Month) && date.Year.Equals(black.Year)) {
                        AddBlackoutDates(facility, month);
                    }
                } else {
                    AddSelectedDates(facility, month);
                    AddBlackoutDates(facility, month);
                }
            }
        }

        /// <summary>
        ///     This is a helper method that highlights a facility's proposed inspection date in the calendar object.
        /// </summary>
        /// <param name="facility">The facility to pull the date from.</param>
        /// <param name="month">The Calendar object the date should be added to.</param>
        public static void AddSelectedDates(Facility facility, Calendar month)
        {
            month.SelectedDates.Add(DateTime.Parse(facility.ProposedDateString));
        }

        /// <summary>
        ///     This is a helper method that 'X's through a facility's most recent inspection date in the calendar object.
        /// </summary>
        /// <param name="facility">The facility to pull the date from.</param>
        /// <param name="month">The Calendar object the date should be added to.</param>
        public static void AddBlackoutDates(Facility facility, Calendar month)
        {
            month.BlackoutDates.Add(
                new CalendarDateRange(DateTime.Parse(facility.MostRecentFullInspectionString)));
        }
    }
}