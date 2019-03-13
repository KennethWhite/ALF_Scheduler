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

        /// <value>Gets the Facility's most recent full inspection information.</value>
        public Inspection MostRecentFullInspection => LastFullInspection();

        /// <value>Gets the Facility's most recent full inspection information as a short string.</value>
        public string MostRecentFullInspectionString => LastFullInspection().InspectionDate.ToShortDateString();

        /// <value>Gets the Facility's most recent full inspection information as a formatted string.</value>
        public string MostRecentFullInspectionFormatString => GetDateString(LastFullInspection().InspectionDate);

        /// <value>Gets the Facility's proposed inspection information as a formatted string.</value>
        public string ProposedDateFormatString => GetDateString(ProposedDate);

        /// <summary>
        /// Gets a date in string format.
        /// </summary>
        /// <param name="date">Date to return</param>
        /// <returns>Returns empty string if date is empty (01/01/0001)</returns>
        public static string GetDateString(DateTime date)
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
        public Inspection TwoYearFullInspection => NthPreviousInspection(2);
 
        /// <value>Gets the inspection result for the facility.</value>
        public string InspectionResult {
            get
            {
                if (MostRecentFullInspection != null) {
                    if (MostRecentFullInspection.Code != null)
                    {
                        if (!string.IsNullOrWhiteSpace(MostRecentFullInspection.Code.Name))
                        {
                            return MostRecentFullInspection.Code.Name;
                        }
                    }
                }
                return "";
            }
        }

        /// <value>Gets the number of beds in the facility.</value>
        public int NumberOfBeds { get; set; }

        /// <value>Gets the Facility's complaints.</value>
        public string Complaints { get; set; }
        
        /// <value>Gets the dates of SOD (statement of deficiencies report) for complaints done since the last inspection.</value>
        //public string DatesOfSOD { get; set; }
        
        /// <value>
        ///     Gets the Facility's enforcement notes (fines, stop placement, conditions, revocation, summary suspension) since
        ///     last inspection.
        /// </value>
        public string EnforcementNotes { get; set; }

        /// <value>Gets the proposed inspection date.</value>
        public DateTime ProposedDate { get; set; }

        /// <value>Gets the proposed inspection date as a short string.</value>
        public string ProposedDateString { get => GetDateString(ProposedDate); }

        /// <value>Gets the time interval between the most recent inspection, and the proposed date.</value>
        /// <remarks>The difference in time is calculated by multiplying the difference in days by the average months in a year.</remarks>
        public float ScheduleInterval
        {
            get
            {                
                if(!ProposedDate.Equals(new DateTime()) && HasInspection())
                {
                    var difference = ProposedDate.Subtract(MostRecentFullInspection.InspectionDate);
                    return (float)(difference.TotalDays / 30.42);
                }

                if (!ProposedDate.Equals(new DateTime()) && !HasInspection())
                    return (float)(ProposedDate.Subtract(DateTime.Today).TotalDays / 30.42);

                return 0;
            }
        }

        /// <value>Gets the date 15 months from the most recent inspection.</value>
        public DateTime Month15 => MostRecentFullInspection.InspectionDate.AddMonths(15);

        /// <value>Gets the cutoff date 18 months from the most recent inspection.</value>
        public DateTime Month18 => MostRecentFullInspection.InspectionDate.AddMonths(18);

        /// <value>Gets the string of inspectors from the most recent inspection.</value>
        public string Licensors { get; set; }

        /// <value>Gets any special information listed for the facility.</value>
        public string SpecialInfo { get; set; }

        /// <summary>
        /// Gets the Facility's most recent full inspection information.
        /// </summary>
        /// <returns>The most recent <see cref="Inspection"/> date.</returns>
        private Inspection LastFullInspection()
        {
            return NthPreviousInspection(0);
        }

        /// <summary>
        /// Gets the Facility's full inspection information from a year ago.
        /// </summary>
        /// <returns>The nth <see cref="Inspection"/> date.</returns>
        private Inspection NthPreviousInspection(int n)
        {
            /*if (AllInspections.Count == 0)
                return new Inspection();
            else*/
            if (!AllInspections.Any())
                //return new Inspection();
                return Inspection.Inspection_Default;
            var sortList = AllInspections;
            sortList.Sort((i1, i2) => i2.InspectionDate.CompareTo(i1.InspectionDate));

            //Check index out of bounds, if true, get first
            if (n > AllInspections.Count - 1)
                return Inspection.Inspection_Default;

            return sortList.ElementAt(n);
        }

        /// <summary>
        /// This method adds an inspection to <see cref="AllInspections"/>
        /// </summary>
        public void AddInspection(Inspection toAdd)
        {
            AllInspections.Add(toAdd);
        }

        public bool HasInspection()
        {
            if (AllInspections.Count == 1)
                if (AllInspections.ElementAt(0).Equals(Inspection.Inspection_Default))
                    return false;

            if (!AllInspections.Any())
                return false;

            return true;
        }


        /// <summary>
        /// ReturnFacility will return a List<object> of all of the passed in Facilitiy's values, except AddInspection, and NthPreviousInspection.
        /// </summary>
        /// <param name="fac"></param>
        /// <returns>A list of facility properties as strings.</returns>
        public static List<string> ReturnFacility(Facility fac)
        {
            if (fac.MostRecentFullInspection.Equals(Inspection.Inspection_Default))//this will only trigger when a new facility is added, thus there are no prior inspections.
            {
                List<string> f1 = new List<string> {
                fac.FacilityName,
                fac.NameOfLicensee,
                fac.LicenseNumber.ToString(),
                fac.Unit,
                fac.City,
                fac.ZipCode,
                GetDateString(fac.ProposedDate),
                //fac.InspectionResult,
                //fac.EnforcementNotes,
                //fac.ScheduleInterval.ToString(),
                //GetDateString(fac.MostRecentFullInspection.InspectionDate),
                //GetDateString(fac.PreviousFullInspection.InspectionDate),
                //GetDateString(fac.TwoYearFullInspection.InspectionDate),
                //GetDateString(fac.Month15),
                //GetDateString(fac.Month18),
                fac.NumberOfBeds.ToString(),
                fac.SpecialInfo,
                fac.Licensors,
                fac.Complaints,
                //fac.DatesOfSOD,
                fac.HasInspection().ToString()
                };
                return f1;
            }
               List<string> f = new List<string> {
                fac.FacilityName,
                fac.NameOfLicensee,
                fac.LicenseNumber.ToString(),
                fac.Unit,
                fac.City,
                fac.ZipCode,
                GetDateString(fac.ProposedDate),
                fac.InspectionResult,
                fac.EnforcementNotes,
                fac.ScheduleInterval.ToString(),
                GetDateString(fac.MostRecentFullInspection.InspectionDate),
                GetDateString(fac.PreviousFullInspection.InspectionDate),
                GetDateString(fac.TwoYearFullInspection.InspectionDate),
                GetDateString(fac.Month15),
                GetDateString(fac.Month18),
                fac.NumberOfBeds.ToString(),
                fac.SpecialInfo,
                fac.Licensors,
                fac.Complaints,
                //fac.DatesOfSOD,
                fac.HasInspection().ToString()
            };

            return f;

        }

        /// <summary>
        /// ReturnFacility will return a List<string> of all of the Facilitiy's values, except AddInspection, and NthPreviousInspection.
        /// </summary>
        /// <returns>A list of facility properties as strings except AddInspection, and NthPreviousInspection.</returns>
        public List<string> ReturnFacility()
        {
            return ReturnFacility(this);
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