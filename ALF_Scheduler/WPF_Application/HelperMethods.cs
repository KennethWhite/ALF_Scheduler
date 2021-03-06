﻿using ALF_Scheduler;
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
            if (facility.ProposedDate.Equals(new DateTime()))
            {
                return;
            }
            month.SelectedDates.Add(facility.ProposedDate);
        }

        /// <summary>
        ///     This is a helper method that 'X's through a facility's most recent inspection date in the calendar object.
        /// </summary>
        /// <param name="facility">The facility to pull the date from.</param>
        /// <param name="month">The Calendar object the date should be added to.</param>
        public static void AddBlackoutDates(Facility facility, Calendar month)
        {
            DateTime date = DateTime.Parse("01/01/0001");
            if (facility.MostRecentFullInspection != null && !facility.MostRecentFullInspection.InspectionDate.Equals(date))
            {
                month.BlackoutDates.Add(
                new CalendarDateRange(DateTime.Parse(facility.MostRecentFullInspectionString)));
            }            
        }

        /// <summary>
        ///     This is a helper method that clears and refreshes all calendar objects selected dates to make sure it has accurate data.
        /// </summary>
        /// <param name="month">The month Calendar object so it can be refreshed as well.</param>
        internal static void RefreshCalendars(Calendar month) {
            month.SelectedDates.Clear();
            App.CalendarYearPage.calendarGrid.Children.RemoveRange(1, App.CalendarYearPage.calendarGrid.Children.Count - 1);
            App.CalendarYearPage.CreateCalendars(DateTime.Today.Year);
            DateSelection(month);
        }
    }
}