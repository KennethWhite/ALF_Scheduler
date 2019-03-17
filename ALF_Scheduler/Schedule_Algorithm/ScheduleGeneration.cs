using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ALF_Scheduler.Models;

namespace ScheduleGeneration
{
    public static class ScheduleGeneration
    {
        /// <summary>
        /// Used to hold a date of a proposed inspection, as well as the months the algorithm selected base off the previous Inspection
        /// </summary>
        private struct NextInspectionDate
        {
            public DateTime Date { get; }
            public double Months { get; }

            public NextInspectionDate(DateTime date, double months)
            {
                Date = date;
                Months = months;
            }
        }

        /// <summary>
        /// Gets the average schedule interval for a list of Facilities.
        /// </summary>
        /// <param name="facilityList">List of facilities to get average from.</param>
        /// <returns>Double representing the average of all the schedule intervals.</returns>
        public static double GetGlobalAverage(ObservableCollection<Facility> facilityList)
        {
            double total = 0;
            double count = 0;

            foreach (Facility fac in facilityList)
            {
                total += fac.ScheduleInterval;
                if (fac.ScheduleInterval != 0)
                    count++;
            }

            if (count != 0)
                return Math.Round(total / count, 2);
            else
                return 0;
        }

        /// <summary>
        /// Main driver of the method, give it a list of Facilities and a desired average, and it will return a ScheduleReturn object which contains the average months of the schedule, and a Dictionary of Facilities and their proposed inspection dates
        /// </summary>
        /// <param name="facilityList">List of Facility Objects.</param>
        /// <param name="desiredAvg">Desired average schedule interval.</param>
        /// <returns>ScheduleReturn object, which contains the average of the schedule intervals, and a Key Value dictionary of facilities and their proprosed inspection dates.</returns>
        public static ScheduleReturn GenerateSchedule(ObservableCollection<Facility> facilityList, double desiredAvg)
        {
            double offsetMonths = 0;
            var months = new List<double>();
            var curSize = 0;
            double curSum = 0;
            var daysScheduled = new List<int>();

            var newSchedule = new ScheduleReturn();
            newSchedule.FacilitySchedule = new Dictionary<Facility, DateTime>();

            foreach (var facility in facilityList)
            {
                Inspection lastInspec = facility.MostRecentFullInspection;
                DateTime date_lastInspection;
                Code code_lastInspection;


                if (lastInspec.Equals(Inspection.Inspection_Default))
                {
                    //No prev inspection.
                    code_lastInspection = new Code() { MinMonth = 6, MaxMonth = 9 };
                    date_lastInspection = DateTime.Today;
                }
                else if (!DateRangeAbleToSchedule(lastInspec))
                {
                    goto EndOfLoop;
                }
                else
                {
                    date_lastInspection = lastInspec.InspectionDate;
                    code_lastInspection = lastInspec.Code;
                }

                NextInspectionDate newNextInspection;

                if (offsetMonths < 0.01 && offsetMonths > -0.01)
                    newNextInspection = PreferRandom(date_lastInspection, code_lastInspection);
                else
                    newNextInspection = PreferAverage(date_lastInspection, code_lastInspection, curSize, curSum,
                        desiredAvg);

                CheckBlackout:
                if (!IsUnscheduledDate(daysScheduled, newNextInspection.Date.DayOfYear) ||
                    IsDateWithinTwoWeeksOfLast(date_lastInspection, newNextInspection.Date))
                {
                    newNextInspection = PreferRandom(date_lastInspection, code_lastInspection);
                    goto CheckBlackout;
                }

                CheckDateFuture:
                if (!(newNextInspection.Date > DateTime.Today))
                {
                    newNextInspection = PreferRandom(date_lastInspection, code_lastInspection);
                    goto CheckDateFuture;
                }

                daysScheduled.Add(newNextInspection.Date.DayOfYear);
                newSchedule.FacilitySchedule.Add(facility, newNextInspection.Date);
                months.Add(newNextInspection.Months);

                curSize = months.Count;
                curSum = 0;

                foreach (var i in months) curSum += i;

                var curAvg = Math.Round(curSum / curSize, 2);
                offsetMonths = Math.Round(curAvg - desiredAvg, 2);
                EndOfLoop: ;
            }

            var size = months.Count;
            double sum = 0;

            foreach (var i in months) sum += i;

            newSchedule.GlobalAvg = Math.Round(sum / size, 2);

            return newSchedule;
        }

        /// <summary>
        /// Generates a date for a proposed inspection. Uses random chance to generate a proposed date given the Facility's last inspection. If no previous inspection, will generate random month between 6 and 9 months.
        /// </summary>
        /// <param name="facility">The facility to generate a proposed date for.</param>
        /// <param name="facList"></param>
        /// <returns></returns>
        public static DateTime GenerateSingleDate(Facility facility, ObservableCollection<Facility> facList)
        {
            Inspection lastInspec = facility.MostRecentFullInspection;
            DateTime date_lastInspection;
            Code code_lastInspection;
            bool lastInspecNull = false;

            if (lastInspec.Equals(Inspection.Inspection_Default))
            {
                //No prev inspection. Generate random 6-9 months out.
                code_lastInspection = new Code() { MinMonth = 6, MaxMonth = 9};
                date_lastInspection = DateTime.Today;
                lastInspecNull = true;
                //return PreferRandom(date_lastInspection, code_lastInspection).Date;
            }
            else
            {
                date_lastInspection = lastInspec.InspectionDate;
                code_lastInspection = lastInspec.Code;
            }

            /* This is if we want to grab a date to perfectly align with average
            double best = GetClosestToValue(code_lastInspection.MinMonth, code_lastInspection.MaxMonth, desiredAvg);
            int days = MonthsToDays(best);

            return date_lastInspection.AddDays(days);
            */

            if (!lastInspecNull && !DateRangeAbleToSchedule(lastInspec))
            {
                MessageBox.Show("The last inspection was on: " + date_lastInspection + ". With code: " + code_lastInspection.Name + ". The maximum date to schedule is: " +
                    date_lastInspection.AddMonths(code_lastInspection.MaxMonth) + ", which is in the past. New date scheduled needs to be manually entered. Scheduling today.",
                    "No future date warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return DateTime.Today;
            }

            GenNew:

            DateTime newDate = PreferRandom(date_lastInspection, code_lastInspection).Date;

            if(!lastInspecNull)
            {
                if (IsDateWithinTwoWeeksOfLast(date_lastInspection, newDate))
                    goto GenNew;
            }

            List<int> scheduledDays = new List<int>();
            foreach(Facility fac in facList)
            {
                var propDate = fac.ProposedDate;
                if (!propDate.Equals(new DateTime()))
                    scheduledDays.Add(propDate.DayOfYear);
            }

            if (!IsUnscheduledDate(scheduledDays, newDate.DayOfYear) || !(newDate > DateTime.Today))
                goto GenNew;

            return newDate;
        }

        /// <summary>
        /// Checks if a date is within +/- 2 weeks of other date.
        /// </summary>
        /// <param name="prevInspection">Date to add +/- 2 weeks to.</param>
        /// <param name="nextInspection">Date to check if within +/- 2 weeks.</param>
        /// <returns>Boolean whether or not within +/- 2 weeks of date.</returns>
        private static bool IsDateWithinTwoWeeksOfLast(DateTime prevInspection, DateTime nextInspection)
        {
            return nextInspection < prevInspection.AddDays(14) && nextInspection > prevInspection.AddDays(-14);
        }

        /// <summary>
        /// Used to check conflicts between a new proposed inspection and the list contain previous proposed inspection. Default is 6 days past date. Incoming dates must be in int format represented as its DateTime.DayOfYear
        /// </summary>
        /// <param name="scheduledDays">List of ints representing a day of the year.</param>
        /// <param name="startInspection">Int representing a day of the yeay.</param>
        /// <returns>Boolean whether or not within 6 days of another date.</returns>
        private static bool IsUnscheduledDate(List<int> scheduledDays, int startInspection)
        {
            var notScheduled = true;
            var endInspection = startInspection + 6;

            var newInpectionRange = GetRange(startInspection, endInspection);

            foreach (var startDay in scheduledDays)
            {
                var endDay = startDay + 6;
                var currentRange = GetRange(startDay, endDay);

                if (currentRange.Intersect(newInpectionRange).Any()) notScheduled = false;
            }

            return notScheduled;
        }

        /// <summary>
        /// Provides a list of ints given a start and end value. Both inclusive
        /// </summary>
        /// <param name="start">Start int.</param>
        /// <param name="end">End int.</param>
        /// <returns>List of ints within range. Inclusive.</returns>
        private static List<int> GetRange(int start, int end)
        {
            var list = new List<int>();

            for (var i = start; i <= end; i++) list.Add(i);

            return list;
        }

        /// <summary>
        /// Takes the date of a previous Inspection and the Code of the previous Inspection, and randomly chooses a month within the code's allowed range
        /// </summary>
        /// <param name="lastInspection">DateTime of previous inspection.</param>
        /// <param name="lastCode">Result code of previous inspection.</param>
        /// <returns>NextInspectionDate object, containing new date and the months in between the last inspection.</returns>
        private static NextInspectionDate PreferRandom(DateTime lastInspection, Code lastCode)
        {
            var minMonth = lastCode.MinMonth;
            var maxMonth = lastCode.MaxMonth;

            var months = Math.Round(NextDoubleInRange(minMonth, maxMonth), 2);
            var daysBetween = MonthsToDays(months);

            var newDate = lastInspection.AddDays(daysBetween);

            return new NextInspectionDate(newDate, months);
        }

        /// <summary>
        /// Takes the date and Code of the last Inspection, as well as the current count of months generated (totMonths), the sum of each of those months (monthSum), and the desired average. It then calculates the best value within the given Code's range of months to get the desired average
        /// </summary>
        /// <param name="date">Date of last inspection.</param>
        /// <param name="code">Result code of last inspection.</param>
        /// <param name="totMonths">Count of the total months generated.</param>
        /// <param name="monthSum">The sum of all the months generated.</param>
        /// <param name="desiredAvg">The average schedule interval to attain to.</param>
        /// <returns>NextInspectionDate object, containing new date and the months in between the last inspection.</returns>
        private static NextInspectionDate PreferAverage(DateTime date, Code code, int totMonths,
            double monthSum, double desiredAvg)
        {
            var minMonth = code.MinMonth;
            var maxMonth = code.MaxMonth;

            var bestMonth = desiredAvg * (totMonths + 1) - monthSum;

            var months = Math.Round(GetClosestToValue(minMonth, maxMonth, bestMonth), 2);

            var daysBetween = MonthsToDays(months);

            var newDate = date.AddDays(daysBetween);

            return new NextInspectionDate(newDate, months);
        }

        /// <summary>
        /// Used to get the closest value as a double given a minimum and maximum value permitted.
        /// </summary>
        /// <param name="min">Int at the bottom of the range.</param>
        /// <param name="max">Int at the top of the range.</param>
        /// <param name="goal">Double as preffered value to get.</param>
        /// <returns>A double that is closest to the goal value.</returns>
        private static double GetClosestToValue(int min, int max, double goal)
        {
            double value;
            if (goal <= min)
                value = min;
            else if (goal >= max)
                value = max;
            else
                value = goal;

            return value;
        }

        /// <summary>
        /// Returns the approximate month value given the days and assuming the average days per month is 30.42
        /// </summary>
        /// <param name="days">Days to turn into months.</param>
        /// <returns>Double represent the months from given days.</returns>
        private static double DaysToMonths(double days)
        {
            return days / 30.42;
        }

        /// <summary>
        /// Returns the approximate day value given the month value and assuming the average days per month is 30.42
        /// </summary>
        /// <param name="months">Montht to turn into days.</param>
        /// <returns>Int representing the days from given months.</returns>
        private static int MonthsToDays(double months)
        {
            return Convert.ToInt32(months * 30.42);
        }

        /// <summary>
        /// Returns a random double from within a range of given doubles
        /// </summary>
        /// <param name="min">Lower boundary.</param>
        /// <param name="max">Upper boundary.</param>
        /// <returns>Random double from range of values.</returns>
        private static double NextDoubleInRange(double min, double max)
        {
            var random = new Random();
            var next = random.NextDouble();

            return next * (max - min) + min;
        }

        /// <summary>
        /// Returns an array of DateTimes of all the in between two dates. Both Inclusive
        /// </summary>
        /// <param name="startDate">Date to start list at.</param>
        /// <param name="endDate">Date to end list at.</param>
        /// <returns>An array of DateTimes containing all the dates between given range.</returns>
        public static DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            var allDates = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }

        /// <summary>
        /// This method returns a list of all facilities with inspections occurring within the next <paramref name="months"/>
        /// </summary>
        /// <param name="facilities">The complete list of Facilities to earch through</param>
        /// <param name="months">The number of months from the current date to use as a cutoff</param>
        /// <returns></returns>
        public static List<Facility> RetrieveFacilitiesWithInspectionsInXMonths(ObservableCollection<Facility> facilities, int months = 6)
        {
            List<Facility> upcomingInspections = facilities.Where(fac => fac.ProposedDate.Subtract(DateTime.Today).TotalDays < months * 31).ToList();
            return upcomingInspections;
        }

        private static bool DateRangeAbleToSchedule(Inspection inspection)
        {
            try
            {
                DateTime lastInspec = inspection.InspectionDate;
                int maxMonth = inspection.Code.MaxMonth;

                return lastInspec.AddMonths(maxMonth) > DateTime.Today;
            }
            catch
            {
                return false;
            }            
        }

        
    }
}