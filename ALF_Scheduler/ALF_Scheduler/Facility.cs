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
        public string InspectionResult { get; set; }

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

        /// <value>Gets the time interval between the most recent inspection, and the proposed date as a float.</value>
        /// <remarks>The difference in time is calculated by dividing the difference in days by the average months in a year.</remarks>
        public float ScheduleInterval {
            get {
                TimeSpan difference = DateTime.Parse(ProposedDate).Subtract(DateTime.Parse(MostRecentFullInspection));
                return (float)(difference.TotalDays/30.42);
            }
        }

        /// <value>Gets the date 15 months from the most recent inspection as a string.</value>
        public string Month15 { get => DateTime.Parse(MostRecentFullInspection).AddMonths(15).ToShortDateString(); }

        /// <value>Gets the cutoff date 18 months from the most recent inspection as a string.</value>
        public string Month18 { get => DateTime.Parse(MostRecentFullInspection).AddMonths(18).ToShortDateString(); }

        // TODO double check this is what's meant by number of licensors
        /// <value>Gets the number of licensors needed based on bed count for this facility's inspection.</value>
        public int NumberOfLicensors { get; set; }
        
        /// <value>Gets the sample size of inspectors from the most recent inspection.</value>
        public int SampleSize { get => LicensorList.Length; }

        /// <value>Gets the string of inspectors from the most recent inspection.</value>
        public string[] LicensorList { get; set; }

        /// <value>Gets any special information listed for the facility.</value>
        public string SpecialInfo { get; set; }
    }
}
