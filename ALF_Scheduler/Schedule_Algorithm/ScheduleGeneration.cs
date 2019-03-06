using System;
using System.Collections.Generic;
using System.Linq;
using ALF_Scheduler;
using ALF_Scheduler.Models;

namespace ScheduleGeneration
{
    public class ScheduleGeneration
    {
        //Used to hold a date of a proposed inspection, as well as the months the algorithm selected base off the previous Inspection
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

        public static double GetGlobalAverage(List<Facility> facilityList)
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

        //Main driver of the method, give it a list of Facilities and a desired average, and it will return a ScheduleReturn object which contains the average months of the schedule, and a Dictionary of Facilities and their proposed inspection dates
        public static ScheduleReturn GenerateSchedule(List<Facility> facilityList, double desiredAvg)
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
                var lastInspec = facility.MostRecentFullInspection;
                var date_lastInspection = lastInspec.InspectionDate;
                var code_lastInspection = lastInspec.Code;


                if (lastInspec == null || date_lastInspection == null || code_lastInspection == null || date_lastInspection.Equals(new DateTime()) || code_lastInspection.Equals(new Code()))
                {
                    goto EndOfLoop;
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

        public static DateTime GenerateSingleDate(Facility facility, double curAvg, double desiredAvg, List<Facility> facList)
        {
            var lastInspec = facility.MostRecentFullInspection;
            var date_lastInspection = lastInspec.InspectionDate;
            var code_lastInspection = lastInspec.Code;
            bool lastInspecNull = false;

            if (lastInspec == null || date_lastInspection == null || code_lastInspection == null || date_lastInspection.Equals(new DateTime()) || code_lastInspection.Equals(new Code()))
            {
                //No prev inspection. Generate random 6-9 months out.
                code_lastInspection = new Code();
                code_lastInspection.MinMonth = 6;
                code_lastInspection.MaxMonth = 9;
                date_lastInspection = DateTime.Now;
                lastInspecNull = true;
                //return PreferRandom(date_lastInspection, code_lastInspection).Date;
            }

            /* This is if we want to grab a date to perfectly align with average
            double best = GetClosestToValue(code_lastInspection.MinMonth, code_lastInspection.MaxMonth, desiredAvg);
            int days = DoubleMonthToDays(best);

            return date_lastInspection.AddDays(days);
            */

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

            if (!IsUnscheduledDate(scheduledDays, newDate.DayOfYear))
                goto GenNew;

            return newDate;
        }

        private static bool IsDateWithinTwoWeeksOfLast(DateTime prevInspection, DateTime nextInspection)
        {
            return nextInspection < prevInspection.AddDays(14) && nextInspection > prevInspection.AddDays(-14);
        }

        //Used to check conflicts between a new proposed inspection and the list contain previous proposed inspection. Incoming dates must be in int format represented as its DateTime.DayOfYear
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

        //Provides a list of ints given a start and end value. Both inclusive
        private static List<int> GetRange(int start, int end)
        {
            var list = new List<int>();

            for (var i = start; i <= end; i++) list.Add(i);

            return list;
        }

        //Takes the date of a previous Inspection and the Code of the previous Inspection, and randomly chooses a month within the code's allowed range
        private static NextInspectionDate PreferRandom(DateTime lastInspection, Code lastCode)
        {
            var minMonth = lastCode.MinMonth;
            var maxMonth = lastCode.MaxMonth;

            var months = Math.Round(NextDoubleInRange(minMonth, maxMonth), 2);
            var daysBetween = DoubleMonthToDays(months);

            var newDate = lastInspection.AddDays(daysBetween);

            return new NextInspectionDate(newDate, months);
        }

        //Takes the date and Code of the last Inspection, as well as the current count of months generated (totMonths), the sum of each of those months (monthSum), and the desired average. It then calculates the best value within the given Code's range of months to get the desired average
        private static NextInspectionDate PreferAverage(DateTime date, Code code, int totMonths,
            double monthSum, double desiredAvg)
        {
            var minMonth = code.MinMonth;
            var maxMonth = code.MaxMonth;

            var bestMonth = desiredAvg * (totMonths + 1) - monthSum;

            var months = Math.Round(GetClosestToValue(minMonth, maxMonth, bestMonth), 2);

            var daysBetween = DoubleMonthToDays(months);

            var newDate = date.AddDays(daysBetween);

            return new NextInspectionDate(newDate, months);
        }

        //Used to get the closest value as a double given a minimum and maximum value permitted.
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

        //Returns the approximate month value given the days and assuming the average days per month is 30.42
        private static double DaysToDoubleMonth(double days)
        {
            return days / 30.42;
        }

        //Returns the approximate day value given the month value and assuming the average days per month is 30.42
        private static int DoubleMonthToDays(double months)
        {
            return Convert.ToInt32(months * 30.42);
        }

        //Returns a random double from within a range of given doubles
        private static double NextDoubleInRange(double min, double max)
        {
            var random = new Random();
            var next = random.NextDouble();

            return next * (max - min) + min;
        }

        //Returns an array of DateTimes of all the in between two dates. Both Inclusive
        public static DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            var allDates = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }
    }
}