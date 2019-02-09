using ALF_Scheduler.Domain.Models;
using ALF_Scheduler.Models;
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
    public class Facility : Entity
    {
        private List<Inspection> AllInspections { get; set; }

        
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
        public int LicenseNumber { get => _LicenseNumber;
            set
            {
                _LicenseNumber = Convert.ToInt32(value);
            }
        }
        private int _LicenseNumber;

        /// <value>Gets the Facility's unit.</value>
        public string Unit { get; set; }
        
        /// <value>Gets the Facility's address.</value>
        public string City { get; set; }

        /// <value>Gets the Facility's zipcode.</value>
        public string ZipCode { get; set; }

        /// <value>Gets the number of beds in the facility.</value>
        public int NumberOfBeds { get; set; }

        /// <value>Gets the Facility's most recent full inspection information.</value>
        public Inspection MostRecentFullInspection { get => LastFullInspection(); }

        /// <value>Gets the Facility's full inspection information from a year ago.</value>
        public Inspection PreviousFullInspection { get => PreviousInspection();}

        /// <value>Gets the Facility's full inspection date from two years ago.</value>
        public Inspection TwoYearFullInspection { get => TwoInspectionsPrevious();}

        //TODO connect inspection results with config file (NO, NO24, ENF, YES) 
        /// <value>Gets the inspection result for the facility.</value>
        public string InspectionResult { get; set; }

        //TODO dates of SOD (Statement of Deficiencies report)
        /// <value>Gets the dates of SOD (statement of deficiencies report) for complaints done since the last inspection.</value>
        public DateTime DatesOfSOD { get; set; }

        // TODO enforcement notes
        /// <value>Gets the Facility's enforcement notes (fines, stop placement, conditions, revocation, summary suspension) since last inspection.</value>
        public string EnforcementNotes { get; set; }

        // TODO failed follow up. Datatype for this??
        /// <value>Gets the value for failed follow ups.</value>
        public DateTime FailedFollowUp { get; set; }

        // TODO complaints
        /// <value>Gets the Facility's complaints.</value>
        public string Complaints { get; set; }

        /// <value>Gets the proposed inspection date.</value>
        public DateTime ProposedDate { get; set; }

        /// <value>Gets the time interval between the most recent inspection, and the proposed date.</value>
        /// <remarks>The difference in time is calculated by multiplying the difference in days by the average months in a year.</remarks>
        public float ScheduleInterval
        {
            get {
                TimeSpan difference = ProposedDate.Subtract(MostRecentFullInspection.InspectionDate);
                return (float)(difference.TotalDays / 30.42);
            }
        }

        /// <value>Gets the date 15 months from the most recent inspection.</value>
        public DateTime Month15 { get => MostRecentFullInspection.InspectionDate.AddMonths(15); }

        /// <value>Gets the cutoff date 18 months from the most recent inspection.</value>
        public DateTime Month18 { get => MostRecentFullInspection.InspectionDate.AddMonths(18); }

        // TODO double check this is what's meant by number of licensors
        /// <value>Gets the number of licensors needed based on bed count for this facility's inspection.</value>
        public int NumberOfLicensors { get; set; }

        /// <value>Gets the sample size of inspectors from the most recent inspection.</value>
        public int SampleSize { get => LicensorList.Length; }

        /// <value>Gets the string of inspectors from the most recent inspection.</value>
        public string LicensorList { get; set; }

        /// <value>Gets any special information listed for the facility.</value>
        public string SpecialInfo { get; set; }

        
        public void ParseLicenseeIntoFirstLastName(string licensee)
        {
            if (licensee.Contains(","))
            {
                string[] firstLast = licensee?.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                LicenseeLastName = firstLast[0];
                LicenseeFirstName = firstLast[1];
            }
            else
            {
                LicenseeLastName = licensee;
            }
           
        }

        /// <value>Gets the Facility's most recent full inspection information.</value>
        private Inspection LastFullInspection()
        {
            if (!AllInspections.Any())
            {
                throw new NullReferenceException("Facility List is Empty");
            }
            List<Inspection> sortList = AllInspections;
            sortList.Sort((i1, i2) => i1.InspectionDate.CompareTo(i2.InspectionDate));
            return sortList.ElementAt(0);
        }

        /// <value>Gets the Facility's full inspection information from a year ago.</value>
        private Inspection PreviousInspection()
        {
            throw new NotImplementedException();
        }

        /// <value>Gets the Facility's full inspection date from two years ago.</value>
        private Inspection TwoInspectionsPrevious()
        {
            throw new NotImplementedException();
        }
    }
}
