using System;
using System.Globalization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//Calculating difference between months code was borrowed from here: https://dotnetcodr.com/2015/10/30/calculate-the-number-of-months-between-two-dates-with-c/
namespace ScheduleGenerator
{
    public class ScheduleAlgorithm
    {
        public static void start()
        {
            Console.WriteLine("Enter number of inspections to run: ");
            int choice = Convert.ToInt32(Console.ReadLine());
            List<String> codeList = new List<String>();
            List<DateTime> lastVisit = new List<DateTime>();
            List<DateTime> nextVisit = new List<DateTime>();
            nextVisit = autoRun(lastVisit, choice, codeList);
            Console.WriteLine("Next Visit Dates Pre alterAverage(): ");

            double average = calcAverage(nextVisit);
            int i = 0;
            Console.WriteLine(i + ". Average: " + average);
            while(average < 15.9 || average == 16 || average > 15.9)
            {
                if (i>=999)
                    break;


                i++;
                average = alterAverage(nextVisit, codeList, lastVisit);
                Console.WriteLine(i + ". Average: " + average);
            }

            for (int j = 0; j < lastVisit.Count; j++)
            {
                Console.Write("Last Visit: " + lastVisit[j]);
                Console.WriteLine("----> Next Visit: " + nextVisit[j]);
            }
            Console.WriteLine("Final Average: " + calcAverage(nextVisit));
        }

        public static double alterAverage(List<DateTime> nextVisit,List<String> codeList, List<DateTime> lastVisit)//idea is to loop through the List of dates, trying to spread out the days in between visits to make the average go up.
        {
            //on averagee there is 30.42 days in a month.
            double avg = calcAverage(nextVisit);
            double diff = avg - 15.99;//calculate the difference between the average we have and 15.99. Not sure if it is needed, but for right now its good to have an idea of how far off target it is.
            double diffInDays = Math.Abs(diff) * 30.42;
            double diffInDaysDividedByCount = diffInDays / nextVisit.Count();
            if(diffInDaysDividedByCount > 0 && diffInDaysDividedByCount < 1)//if the diffInDaysDiviedByCount is a decimal, then it is small enough to call it quits.
            {
                return 0.0;
            }
            Console.WriteLine("diff in days: " + diffInDays);
            Console.WriteLine("diff in days / List Count = " + diffInDaysDividedByCount);

            if (diff < 0)//checking for if we need to add more days in order to raise the averaeg. Will need a similar check later to see if average is above 15.99 too.
            {
                for (int i = 1; i < nextVisit.Count; i++)
                {
                    Console.WriteLine("Pre Avg Alter: " + nextVisit[i]);
                    int codeCheck = (int)(nextVisit[i] - lastVisit[i]).TotalDays;
                    if (checkCodeBound(codeList[i],codeCheck)== true)
                    {
                        nextVisit[i] = nextVisit[i].AddDays(diffInDaysDividedByCount);//Code in here is what is wonky. Need to figure out how to spread the visits apart more, while still maintaining WA Guidelines.
                    }                        
                    Console.WriteLine("Post Avg Alter: " + nextVisit[i]);
                }
            }
            else
            {
                for (int i = 1; i < nextVisit.Count; i++)
                {
                    Console.WriteLine("Pre Avg Alter: " + nextVisit[i]);
                    nextVisit[i] = nextVisit[i].AddDays(-diffInDaysDividedByCount);//Code in here is what is wonky. Need to figure out how to spread the visits apart more, while still maintaining WA Guidelines.
                    Console.WriteLine("Post Avg Alter: " + nextVisit[i]);
                }
            }
            foreach (DateTime d in nextVisit)
            {
                printDates(d);
            }
            return calcAverage(nextVisit);//calc new average
        }

        public static double calcAverage(List<DateTime> myDates)//https://dotnetcodr.com/2015/10/30/calculate-the-number-of-months-between-two-dates-with-c/
        {
            double average = 0;
            int timeTweenMonths = 0;

            for (int i = 0; i < myDates.Count -1; i++)
            {
                timeTweenMonths = 12 * (myDates[i].Year - myDates[i + 1].Year) + myDates[i].Month - myDates[i + 1].Month + 1;//calculations here borrowed from link on method header line.
                average += timeTweenMonths;
            }
            average = average / myDates.Count;

            return Math.Abs(average);
        }

        public static List<DateTime> autoRun(List<DateTime> lastVisit, int runs, List<String> codeList)
        {
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            DateTime myDt = new DateTime();//this will be the prevous inspection date
            //DateTime futureDate = new DateTime();
            List<DateTime> nextVisit = new List<DateTime>();
            for (int i = 0; i < runs; i++)
            {
                DateTime futureDate = new DateTime();
                Console.WriteLine("Enter date of last inspection (MM/DD/YYYY): ");
                string date = Console.ReadLine();
                while (!DateTime.TryParseExact(date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out myDt))
                {
                    Console.WriteLine("Invalid date, try again");
                    date = Console.ReadLine();
                }
                string resultCode = "";
                int verifyCode = 0;
                Console.WriteLine("Enter Result Code of Inspection: ");
                resultCode = Console.ReadLine();
                verifyCode = isCode(resultCode, myDt);
                codeList.Add(resultCode);
                lastVisit.Add(myDt);

                futureDate = myDt.AddDays(verifyCode);
                Console.WriteLine("Future Date: " + futureDate);
                nextVisit.Add(futureDate);
            }

            return nextVisit;
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
            lowTempDate = pastInspection.AddDays(low);
            highTempDate = pastInspection.AddDays(high);
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

        public static bool checkCodeBound(String code, int diff)//checking to see if the future scheduled date goes beyond the highest bound of the corresponding code.
        {
            if (code.CompareTo("NO24") == 0)
            {
                return (730 - diff) > 0;
            }
            else if (code.CompareTo("NO") == 0)
            {
                return (547 - diff) > 0;//16-18 months in days

            }
            else if (code.CompareTo("YES") == 0)
                return (456 - diff) > 0;//13-15 months in days
            else if (code.CompareTo("ENF") == 0)
                return (365 - diff) > 0;//9-12 months in days
            else if (code.CompareTo("CHOWN") == 0)
                return (273 - diff) > 0;//6-9 months in days

            else
                return true;
        }

    }


}
