using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// This class is used to store the metadata for each facility 
/// </summary>

namespace ALF_Scheduler
{
    class Facility
    {
        public string Name { get; set; }
        public string NameOfLicensee { get; set; }
        public char Unit { get; set; }
        public int LicenseNumber { get; set; }
        public string Address { get; set; }
        public int ZipCode { get; set; }
        public int NumberOfBeds { get; set; }
        public DateTime LastFullInspection { get; set; }
        public DateTime OneYearFullInspection { get; set; } //need to have better names for previous inspection dates and enforcement
        public DateTime TwoYearFullInspection { get; set; } //need to have better names for previous inspection dates and enforcement
        public string InspectionResults { get; set; } //TODO connect inspection results with config file (NO, NO24, ENF, YES) 
        public DateTime[] DatesOfSOD { get; set; } //TODO dates of SOD for substantiated complaints done since the last full inspection?
        public string Enforcement { get; set; } //TODO any enforcement for any inspection done since the last full inspection? (fines, stop placement, conditions, revocation, summary suspension)
        public bool FailedFollowUp { get; set; } //datatype for this?
        public string Complaints { get; set; } // failed follow-ups for current inspection or complaints
        public DateTime ProposedDate { get; set; }
        public float ScheduleInterval { get; set; } //time between last full inspection and proposed date
        public DateTime Month15 { get => LastFullInspection.AddMonths(15); } //what 15 months is from the last full inspection
        public DateTime Month18 { get => LastFullInspection.AddMonths(18); } //what 18 months is from the last full inspection
        public int NumberOfLicensors { get; set; } //number of licensors needed.. for inspection I'm guessing?
        public int SampleSize { get; set; } //sample size from last full inspection.. of inspectors?
    }
}
