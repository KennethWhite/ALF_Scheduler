using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This class is used to store the metadata in properties for each facility.
/// </summary>

namespace ALF_Scheduler
{
    class Facility {

        private string _inspectionResult;

        /// <value>Gets the Facility name.</value>
        public string Name { get; set; }

        /// <value>Gets the full name of the licensee separated by a comma (last, first).</value>
        public string NameOfLicensee {
            get {
                FormattableString formattableString = $"{LicenseeLastName}, {LicenseeFirstName}";
                return formattableString.ToString();
            }
        }

        /// <value>Gets the Licensee's first name.</value>
        public string LicenseeFirstName { get; set; }

        /// <value>Gets the Licensee's last name.</value>
        public string LicenseeLastName { get; set; }

        /// <value>Gets the license number.</value>
        public int LicenseNumber { get; set; }

        /// <value>Gets the Facility's unit.</value>
        public string Unit { get; set; }
        
        /// <value>Gets the Facility's address.</value>
        public string City { get; set; }

        /// <value>Gets the Facility's zipcode.</value>
        public int ZipCode { get; set; }

        /// <value>Gets the number of beds in the facility.</value>
        public int NumberOfBeds { get; set; }

        /// <value>Gets the Facility's most recent full inspection date as a string.</value>
        public string MostRecentFullInspection { get; set; }

        /// <value>Gets the Facility's full inspection date from a year ago as a string.</value>
        public string OneYearFullInspection { get; set; }

        /// <value>Gets the Facility's full inspection date from two years ago as a string.</value>
        public string TwoYearFullInspection { get; set; }

        //TODO connect inspection results with config file (NO, NO24, ENF, YES) 
        /// <value>Gets the inspection result for the facility.</value>
        public string InspectionResult {
            get => _inspectionResult;
            set => _inspectionResult = value.ToUpper();
        }

        //TODO dates of SOD (Statement of Deficiencies report)
        /// <value>Gets the dates of SOD (statement of deficiencies report) for complaints done since the last inspection.</value>
        public DateTime[] DatesOfSOD { get; set; }

        // TODO enforcement notes
        /// <value>Gets the Facility's enforcement notes (fines, stop placement, conditions, revocation, summary suspension) since last inspection.</value>
        public string EnforcementNotes { get; set; }

        // TODO failed follow up. Datatype for this??
        /// <value>Gets the value for failed follow ups.</value>
        public DateTime[] FailedFollowUp { get; set; }
        
        /// <value>Gets the Facility's complaints.</value>
        public string Complaints { get; set; }

        /// <value>Gets the proposed inspection date as a string.</value>
        public string ProposedDate { get; set; }

        /// <value>Gets the time interval between the most recent inspection, and the proposed date as a string.</value>
        /// <remarks>The difference in time is calculated by dividing the difference in days by the average months in a year.</remarks>
        public string ScheduleInterval {
            get {
                TimeSpan difference = DateTime.Parse(ProposedDate).Subtract(DateTime.Parse(MostRecentFullInspection));
                return ((float)(difference.TotalDays / 30.42)).ToString("F2");
            }
        }

        /// <value>Gets the date 15 months from the most recent inspection as a string.</value>
        public string Month15 { get => DateTime.Parse(MostRecentFullInspection).AddMonths(15).ToShortDateString(); }

        /// <value>Gets the cutoff date 18 months from the most recent inspection as a string.</value>
        public string Month18 { get => DateTime.Parse(MostRecentFullInspection).AddMonths(18).ToShortDateString(); }

        // TODO double check this is what's meant by number of licensors
        /// <value>Gets the number of licensors needed based on bed count for this facility's inspection.</value>
        public int NumberOfLicensors { get; set; }

        /// <value>Gets the string of inspectors from the most recent inspection.</value>
        public string[] LicensorList { get; set; }
        /// <value>Gets the sample size of inspectors from the most recent inspection.</value>
        public int SampleSize { get => LicensorList.Length; }

        /// <value>Gets any special information listed for the facility.</value>
        public string SpecialInfo { get; set; }

        /// <summary>
        /// returnFacility will return string array of all of the passed in Facilitiy's values. Handy for the Details tab.
        /// </summary>
        /// <param name="fac"></param>
        /// <returns></returns>
        public string[] returnFacility(Facility fac)
        {
            string[] s = new string[23];
            StringBuilder sod = new StringBuilder();
            StringBuilder failedFollow = new StringBuilder();
            StringBuilder licensorList = new StringBuilder();

            s[0] = fac.Name;
            s[1] = fac.NameOfLicensee;
            s[2] = fac.LicenseNumber.ToString();
            s[3] = fac.Unit;
            s[4] = fac.City;
            s[5] = fac.ZipCode.ToString();
            s[6] = fac.NumberOfBeds.ToString();
            s[7] = fac.MostRecentFullInspection;
            s[8] = fac.OneYearFullInspection;
            s[9] = fac.TwoYearFullInspection;
            s[10] = fac.InspectionResult;

            if (fac.DatesOfSOD != null)
            {
                foreach (DateTime dt in fac.DatesOfSOD)
                {
                    sod.Append(dt.ToShortDateString());
                }
                s[11] = sod.ToString();
            }
            else
                s[11] = "No Dates of SOD on record";
            
            s[12] = fac.EnforcementNotes;

            if (fac.FailedFollowUp != null)
            {
                foreach (DateTime dt in fac.FailedFollowUp)
                {
                    failedFollow.Append(dt.ToShortDateString());
                }
                s[13] = failedFollow.ToString();
            }
            else
                s[13] = "No Failed Follow Up on record";

            s[14] = fac.Complaints;
            s[15] = fac.ProposedDate.ToString();
            s[16] = fac.ScheduleInterval;
            s[17] = fac.Month15;
            s[18] = fac.Month18;
            s[19] = fac.NumberOfLicensors.ToString();

            if (fac.LicensorList != null)
                s[20] = fac.SampleSize.ToString();
            else
                s[20] = "Sample Size not on record";

            if (fac.LicensorList != null)
            {
                foreach (String str in fac.LicensorList)
                {
                    licensorList.Append(str);
                }

                s[21] = licensorList.ToString();
            }
            else
                s[21] = "No Licensor List on record";
            
            s[22] = fac.SpecialInfo;
            return s;
        }
    }
}
