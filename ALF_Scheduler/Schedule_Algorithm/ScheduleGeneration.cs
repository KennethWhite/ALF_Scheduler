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

        public struct ScheduleReturn
        {
            public IDictionary<Facility, DateTime> FacilitySchedule { get; set; }

            public double GlobalAvg { get; set; }

            public override string ToString()
            {
                return "Global Average: " + GlobalAvg;
            }
        }

        public static ScheduleReturn GenerateSchedule(List<Facility> facilityList, double desiredAvg)
        {
            double offsetMonths = 0;
            List<double> months = new List<double>();
            int curSize = 0;
            double curSum = 0;

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
                    newNextInspection = PreferNone(date_lastInspection, code_lastInspection);
                }
                else
                {
                    newNextInspection = PreferOffset(date_lastInspection, code_lastInspection, curSize, curSum, desiredAvg);
                }

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

        private static NextInspectionDate PreferNone(DateTime lastInspection, Code lastCode)
        {
            int minMonth = lastCode.MinMonth;
            int maxMonth = lastCode.MaxMonth;

            double months = Math.Round(NextDoubleInRange(minMonth, maxMonth), 2);
            int daysBetween = DoubleMonthToDays(months);

            DateTime newDate = lastInspection.AddDays(daysBetween);

            return new NextInspectionDate(newDate, months);
        }

        private static NextInspectionDate PreferOffset(DateTime lastInspection, Code lastCode, int totMonths, double monthSum, double desiredAvg)
        {
            int minMonth = lastCode.MinMonth;
            int maxMonth = lastCode.MaxMonth;

            double bestMonth = desiredAvg * (totMonths + 1) - monthSum;

            double months = Math.Round(GetClosestToValue(minMonth, maxMonth, bestMonth), 2);

            int daysBetween = DoubleMonthToDays(months);

            DateTime newDate = lastInspection.AddDays(daysBetween);

            return new NextInspectionDate(newDate, months);
        }

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

        private static double DaysToDoubleMonth(double days)
        {
            return days / 30.42;
        }

        private static int DoubleMonthToDays(double months)
        {
            return Convert.ToInt32(months * 30.42);
        }

        private static double NextDoubleInRange(double min, double max)
        {
            Random random = new Random();
            var next = random.NextDouble();

            return next * (max - min) + min;
        }

        public static DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }
    }
}