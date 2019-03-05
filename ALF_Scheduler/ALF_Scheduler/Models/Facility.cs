using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// This class is used to store the metadata in properties for each facility.
/// </summary>

namespace ALF_Scheduler.Models
{
    public class Facility : Entity
    {
        private Inspection _previousFullInspection;
        private List<Inspection> AllInspections { get; set; } = new List<Inspection>();

        /// <value>Gets the Facility name.</value>
        public string FacilityName { get; set; }

        /// <value>Gets the full name of the licensee separated by a comma (last, first).</value>
        public string NameOfLicensee
        {
            get
            {
                FormattableString formattableString = $"{LicenseeLastName}, {LicenseeFirstName}";
                return formattableString.ToString();
            }
        }

        /// <value>Gets the Licensee's first name.</value>
        public string LicenseeFirstName { get; set; }

        /// <value>Gets the Licensee's last name.</value>
        public string LicenseeLastName { get; set; }

        /// <value>Gets the license number.</value>
        public string LicenseNumber { get; set; }

        /// <value>Gets the Facility's unit.</value>
        public string Unit { get; set; }

        /// <value>Gets the Facility's address.</value>
        public string City { get; set; }

        /// <value>Gets the Facility's zipcode.</value>
        public string ZipCode { get; set; }

        /// <value>Gets the number of beds in the facility.</value>
        public int NumberOfBeds { get; set; }

        /// <value>Gets the Facility's most recent full inspection information.</value>
        public Inspection MostRecentFullInspection => LastFullInspection();
        public string MostRecentFullInspectionString => LastFullInspection().InspectionDate.ToShortDateString();

        public string MostRecentFullInspectionFormatString => GetDateString(LastFullInspection().InspectionDate);

        public string ProposedDateFormatString => GetDateString(ProposedDate);

        /// <summary>
        /// Gets a date in string format.
        /// </summary>
        /// <param name="date">Date to return</param>
        /// <returns>Returns empty string if date is empty (01/01/0001)</returns>
        private static string GetDateString(DateTime date)
        {
            var outStr = date.ToString("yyyy/MM/dd");
            if (date.Equals(new DateTime()))
                outStr = "";
            return outStr;
        }

        /// <value>Gets the Facility's full inspection information from a year ago.</value>
        public Inspection PreviousFullInspection
        {
            get => NthPreviousInspection(1);
            set => _previousFullInspection = value;
        }

        /// <value>Gets the Facility's full inspection date from two years ago.</value>
        public Inspection TwoYearFullInspection => NthPreviousInspection(1);

        //TODO connect inspection results with config file (NO, NO24, ENF, YES) 
        /// <value>Gets the inspection result for the facility.</value>
        public string InspectionResult { get; set; }

        //TODO dates of SOD (Statement of Deficiencies report)
        /// <value>Gets the dates of SOD (statement of deficiencies report) for complaints done since the last inspection.</value>
        public DateTime DatesOfSOD { get; set; }

        // TODO enforcement notes
        /// <value>
        ///     Gets the Facility's enforcement notes (fines, stop placement, conditions, revocation, summary suspension) since
        ///     last inspection.
        /// </value>
        public string EnforcementNotes { get; set; }


        // TODO complaints
        /// <value>Gets the Facility's complaints.</value>
        public string Complaints { get; set; }

        /// <value>Gets the proposed inspection date.</value>
        public DateTime ProposedDate { get; set; }
        public string ProposedDateString { get => ProposedDate.ToShortDateString(); }

        /// <value>Gets the time interval between the most recent inspection, and the proposed date.</value>
        /// <remarks>The difference in time is calculated by multiplying the difference in days by the average months in a year.</remarks>
        public float ScheduleInterval
        {
            get
            {
                if(!ProposedDate.Equals(new DateTime()))
                {
                    var difference = ProposedDate.Subtract(MostRecentFullInspection.InspectionDate);
                    return (float)(difference.TotalDays / 30.42);
                }
                return 0;
            }
        }

        /// <value>Gets the date 15 months from the most recent inspection.</value>
        public DateTime Month15 => MostRecentFullInspection.InspectionDate.AddMonths(15);

        /// <value>Gets the cutoff date 18 months from the most recent inspection.</value>
        public DateTime Month18 => MostRecentFullInspection.InspectionDate.AddMonths(18);

        /// <value>Gets the string of inspectors from the most recent inspection.</value>
        public string LicensorList { get; set; }

        /// <value>Gets any special information listed for the facility.</value>
        public string SpecialInfo { get; set; }


        /// <value>Gets the Facility's most recent full inspection information.</value>
        private Inspection LastFullInspection()
        {
            return NthPreviousInspection(0);
        }

        /// <value>Gets the Facility's full inspection information from a year ago.</value>
        private Inspection NthPreviousInspection(int n)
        {
            if (!AllInspections.Any()) throw new InvalidOperationException("Facility List is Empty");
            var sortList = AllInspections;
            sortList.Sort((i1, i2) => i2.InspectionDate.CompareTo(i1.InspectionDate));
            return sortList.ElementAt(n);
        }

        public void AddInspection(Inspection toAdd)
        {
            AllInspections.Add(toAdd);
        }

        /// <summary>
        /// returnFacility will return a List<object> of all of the passed in Facilitiy's values, except AddInspection, and NthPreviousInspection.
        /// </summary>
        /// <param name="fac"></param>
        /// <returns></returns>
        public static List<string> returnFacility(Facility fac)
        {
            List<string> f = new List<string>();

            //string allInspections = ListToString(fac.AllInspections);
            //f.Add(allInspections);

            f.Add(fac.FacilityName);
            f.Add(fac.NameOfLicensee);
            f.Add(fac.LicenseNumber.ToString());
            f.Add(fac.Unit);
            f.Add(fac.City);
            f.Add(fac.ZipCode);
            f.Add(fac.NumberOfBeds.ToString());
            f.Add(GetDateString(fac.MostRecentFullInspection.InspectionDate));
            f.Add(GetDateString(fac.PreviousFullInspection.InspectionDate));
            f.Add(GetDateString(fac.TwoYearFullInspection.InspectionDate));
            f.Add(fac.InspectionResult);
            f.Add(GetDateString(fac.DatesOfSOD));
            f.Add(fac.EnforcementNotes);
            f.Add(fac.Complaints);
            f.Add(GetDateString(fac.ProposedDate));
            f.Add(fac.ScheduleInterval.ToString());
            f.Add(GetDateString(fac.Month15));
            f.Add(GetDateString(fac.Month18));
            f.Add(fac.SpecialInfo);
            f.Add(GetDateString(fac.LastFullInspection().InspectionDate));

            return f;

        }

        /// <summary>
        /// returnFacility will return a List<string> of all of the Facilitiy's values, except AddInspection, and NthPreviousInspection.
        /// </summary>
        public List<string> returnFacility()
        {
            return returnFacility(this);
        }


        private string ListToString(List<Inspection> theList)
        {
            if(theList.Count == 0)
                return "No Record";

            string toReturn = "";
            StringBuilder sb = new StringBuilder(toReturn);
            foreach (Inspection i in theList)
            {
                sb.Append(i.InspectionDate.ToShortDateString() + ", ");
            }
            toReturn = sb.ToString();
            if(toReturn.EndsWith(", "))
                toReturn = toReturn.Remove(toReturn.Length-2);

            return toReturn;
        }
    }
}