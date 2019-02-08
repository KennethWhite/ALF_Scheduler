using System;
using System.Linq;
using System.Collections.Generic;
using ALF_Scheduler;
using ALF_Scheduler.Domain.Models;
using ALF_Scheduler.Models;

namespace ScheduleGeneration
{
    public class ScheduleGeneration
    {

        //Used to hold a date of a proposed inspection, as well as the months the algorithm selected base off the previous Inspection
        private struct NextInspectionDate
        {
            public DateTime Date { get; set; }
            public double Months { get; set; }

            public NextInspectionDate(DateTime date, double months)
            {
                Date = date;
                Months = months;
            }
        }

        //Main driver of the method, give it a list of Facilities and a desired average, and it will return a ScheduleReturn object which contains the average months of the schedule, and a Dictionary of Facilities and their proposed inspection dates
        public static ScheduleReturn GenerateSchedule(List<Facility> facilityList, double desiredAvg)
        {
            double offsetMonths = 0;
            List<double> months = new List<double>();
            int curSize = 0;
            double curSum = 0;
            List<int> daysScheduled = new List<int>();

            ScheduleReturn newSchedule = new ScheduleReturn();
            newSchedule.FacilitySchedule = new Dictionary<Facility, DateTime>();

            foreach (Facility facility in facilityList)
            {
                Inspection lastInspec = facility.MostRecentFullInspection;
                DateTime date_lastInspection = lastInspec.InspectionDate;
                Code code_lastInspection = lastInspec.Code;

                NextInspectionDate newNextInspection;

                if (offsetMonths < 0.01 && offsetMonths > -0.01)
                {
                    newNextInspection = PreferRandom(date_lastInspection, code_lastInspection);
                }
                else
                {
                    newNextInspection = PreferAverage(date_lastInspection, code_lastInspection, curSize, curSum, desiredAvg);
                }

            CheckBlackout:
                if (!IsUnscheduledDate(daysScheduled, newNextInspection.Date.DayOfYear))
                {
                    newNextInspection = PreferRandom(date_lastInspection, code_lastInspection);
                    goto CheckBlackout;
                }

                daysScheduled.Add(newNextInspection.Date.DayOfYear);
                newSchedule.FacilitySchedule.Add(facility, newNextInspection.Date);
                months.Add(newNextInspection.Months);

                curSize = months.Count;
                curSum = 0;

                foreach (double i in months)
                {
                    curSum += i;
                }
                
                double curAvg = Math.Round(curSum / curSize, 2);
                offsetMonths = Math.Round(curAvg - desiredAvg, 2);
            }

            int size = months.Count;
            double sum = 0;

            foreach (double i in months)
            {
                sum += i;
            }

            newSchedule.GlobalAvg = Math.Round(sum / size, 2);

            return newSchedule;
        }

        //Used to check conflicts between a new proposed inspection and the list contain previous proposed inspection. Incoming dates must be in int format represented as its DateTime.DayOfYear
        private static bool IsUnscheduledDate(List<int> scheduledDays, int startInspection)
        {
            bool notScheduled = true;
            int endInspection = startInspection + 6;

            List<int> newInpectionRange = GetRange(startInspection, endInspection);

            foreach (int startDay in scheduledDays)
            {
                int endDay = startDay + 6;
                List<int> currentRange = GetRange(startDay, endDay);

                if (currentRange.Intersect(newInpectionRange).Any())
                {
                    notScheduled = false;
                }
            }

            return notScheduled;
        }

        //Provides a list of ints given a start and end value. Both inclusive
        private static List<int> GetRange(int start, int end)
        {
            List<int> list = new List<int>();

            for (int i = start; i <= end; i++)
            {
                list.Add(i);
            }

            return list;
        }

        //Takes the date of a previous Inspection and the Code of the previous Inspection, and randomly chooses a month within the code's allowed range
        private static NextInspectionDate PreferRandom(DateTime lastInspection, Code lastCode)
        {
            int minMonth = lastCode.MinMonth;
            int maxMonth = lastCode.MaxMonth;

            double months = Math.Round(NextDoubleInRange(minMonth, maxMonth), 2);
            int daysBetween = DoubleMonthToDays(months);

            DateTime newDate = lastInspection.AddDays(daysBetween);

            return new NextInspectionDate(newDate, months);
        }

        //Takes the date and Code of the last Inspection, as well as the current count of months generated (totMonths), the sum of each of those months (monthSum), and the desired average. It then calculates the best value within the given Code's range of months to get the desired average
        private static NextInspectionDate PreferAverage(DateTime lastInspection, Code lastCode, int totMonths, double monthSum, double desiredAvg)
        {
            int minMonth = lastCode.MinMonth;
            int maxMonth = lastCode.MaxMonth;

            double bestMonth = desiredAvg * (totMonths + 1) - monthSum;

            double months = Math.Round(GetClosestToValue(minMonth, maxMonth, bestMonth), 2);

            int daysBetween = DoubleMonthToDays(months);

            DateTime newDate = lastInspection.AddDays(daysBetween);

            return new NextInspectionDate(newDate, months);
        }

        //Used to get the closest value as a double given a minimum and maximum value permitted.
        private static double GetClosestToValue(int min, int max, double goal)
        {
            double value;
            if (goal <= min)
            {
                value = min;
            }
            else if (goal >= max)
            {
                value = max;
            }
            else
            {
                value = goal;
            }

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
            Random random = new Random();
            var next = random.NextDouble();

            return next * (max - min) + min;
        }

        //Returns an array of DateTimes of all the in between two dates. Both Inclusive
        public static DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }
    }
}