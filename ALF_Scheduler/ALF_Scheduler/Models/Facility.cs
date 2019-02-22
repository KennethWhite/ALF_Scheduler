using System;
using System.Collections.Generic;
using System.Linq;
using ALF_Scheduler.Domain.Models;
using ALF_Scheduler.Models;

/// <summary>
/// This class is used to store the metadata in properties for each facility.
/// </summary>

namespace ALF_Scheduler
{
    public class Facility : Entity
    {
        private int _LicenseNumber;
        private Inspection _previousFullInspection;
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
        public int LicenseNumber
        {
            get => _LicenseNumber;
            set => _LicenseNumber = Convert.ToInt32(value);
        }

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

        /// <value>Gets the Facility's full inspection information from a year ago.</value>
        public Inspection PreviousFullInspection
        {
            get => NthPreviousInspection(1);
            set => _previousFullInspection = value;
        }

        /// <value>Gets the Facility's full inspection date from two years ago.</value>
        public Inspection TwoYearFullInspection => NthPreviousInspection(2);

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

        /// <value>Gets the time interval between the most recent inspection, and the proposed date.</value>
        /// <remarks>The difference in time is calculated by multiplying the difference in days by the average months in a year.</remarks>
        public float ScheduleInterval
        {
            get
            {
                var difference = ProposedDate.Subtract(MostRecentFullInspection.InspectionDate);
                return (float) (difference.TotalDays / 30.42);
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
            sortList.Sort((i1, i2) => i1.InspectionDate.CompareTo(i2.InspectionDate));
            return sortList.ElementAt(n);
        }

        public void AddInspection(Inspection toAdd)
        {
            AllInspections.Add(toAdd);
        }
    }
}