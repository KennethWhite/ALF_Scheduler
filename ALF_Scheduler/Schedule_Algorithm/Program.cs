﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Algorithm
{
    public static class Utils
    {
        public static void start()
        {
            Console.WriteLine("Enter number of inspections to run: ");
            int choice = Convert.ToInt32(Console.ReadLine());
            List<DateTime> myDates = new List<DateTime>();
            autoRun(myDates, choice);
            double average = calcAverage(myDates);
            int i = 0;
            Console.WriteLine(i + ". Average: " + average);
            if (average < 15.99 || average > 15.99)
            {
                i++;
                average = alterAverage(myDates);
                Console.WriteLine(i + ". Average: " + average);
            }
        }

        public static double alterAverage(List<DateTime> myDates)//idea is to loop through the List of dates, trying to spread out the days in between visits to make the average go up.
        {
            double avg = calcAverage(myDates);
            double diff = avg - 15.99;//calculate the difference between the average we have and 15.99. Not sure if it is needed, but for right now its good to have an idea of how far off target it is.
            Console.WriteLine("diff: " + diff);
            if (diff < 0)//checking for if we need to add more days in order to raise the averaeg. Will need a similar check later to see if average is above 15.99 too.
            {
                for (int i = 0; i < myDates.Count; i++)
                {
                    myDates[i] = myDates[i].AddDays(Math.Abs(diff));//Code in here is what is wonky. Need to figure out how to spread the visits apart more, while still maintaining WA Guidelines.
                    printDates(myDates[i]);
                }
            }
            return calcAverage(myDates);//calc new average
        }
        public static double calcAverage(List<DateTime> myDates)//https://dotnetcodr.com/2015/10/30/calculate-the-number-of-months-between-two-dates-with-c/
        {
            double average = 0;
            int timeTweenMonths = 0;

            for (int i = 0; i < myDates.Count - 1; i++)
            {
                timeTweenMonths = 12 * (myDates[i].Year - myDates[i + 1].Year) + myDates[i].Month - myDates[i + 1].Month;//calculations here borrowed from link on method header line.
                average += timeTweenMonths;
            }
            average = average / myDates.Count;

            return Math.Abs(average);
        }

        public static void autoRun(List<DateTime> myDates, int runs)
        {
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;

            Console.WriteLine("Enter date of last inspection (MM/DD/YYYY): ");
            DateTime myDt = new DateTime();//this will be the prevous inspection date
            //DateTime myFutureDate = new DateTime();
            string date = Console.ReadLine();

            while (!DateTime.TryParseExact(date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out myDt))
            {
                Console.WriteLine("Invalid date, try again");
                date = Console.ReadLine();
            }

            string resultCode = "";
            int verifyCode = 0;
            DateTime tempDate = new DateTime();
            printDates(myDt);//print date format nicely
            myDates.Add(myDt);

            for (int i = 0; i < runs; i++)
            {
                //tempDate = myDt;
                Console.WriteLine("Enter Result Code of Inspection: ");
                resultCode = Console.ReadLine();
                verifyCode = isCode(resultCode, myDates[i]);
                tempDate = myCal.AddDays(myDates[i], verifyCode);
                myDates.Add(tempDate);
                printDates(tempDate);
            }

        }
        public static void printDates(DateTime myDt)
        {
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            Console.Write(myCal.GetDayOfWeek(myDt) + ", ");
            Console.Write(myCal.GetMonth(myDt) + "/");
            Console.Write(myCal.GetDayOfMonth(myDt) + "/");
            Console.WriteLine(myCal.GetYear(myDt) + "\n");
        }

        public static int daysInRange(int low, int high, DateTime pastInspection)//easier to fine tune average when working with adding days, instead of months. ie 16-18 months == 486.667-547.501
        {
            Random rng = new Random();
            int toReturn = rng.Next(low, high);
            DateTime highTempDate;
            DateTime lowTempDate;
            Console.WriteLine("==========================");
            Console.WriteLine("Range of days: " + low + " - " + high);
            Console.WriteLine("Number generated from range: " + toReturn);

            lowTempDate = pastInspection.AddDays(low);
            highTempDate = pastInspection.AddDays(high);
            Console.WriteLine("Range of possible inspection days based off inspection code: " + lowTempDate + " - " + highTempDate);
            Console.WriteLine("==========================");
            return toReturn;
        }

        public static int isCode(string resultCode, DateTime pastInspection)//quick and dirty code checking. Use regex or something stronger later.
        {
            if (resultCode.CompareTo("NO24") == 0)
                return daysInRange(365, 730, pastInspection); //12-24 months in days
            else if (resultCode.CompareTo("NO") == 0)
                return daysInRange(486, 547, pastInspection);//16-18 months in days
            else if (resultCode.CompareTo("YES") == 0)
                return daysInRange(395, 456, pastInspection);//13-15 months in days
            else if (resultCode.CompareTo("ENF") == 0)
                return daysInRange(273, 365, pastInspection);//9-12 months in days
            else if (resultCode.CompareTo("CHOWN") == 0)
                return daysInRange(182, 273, pastInspection);//6-9 months in days
            else
            {
                Console.WriteLine("Invalid Code.");
                return 0; //input error
            }
        }
    }
}