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
        private string name;
        private string nameOfLicensee;
        private char unit;
        private int licenseNumber;
        private string address;
        private int zipCode;
        private int numberOfBeds;
        private DateTime lastFullInspection;
        private DateTime oneYearFullInspection; //need to have better names for previous inspection dates and enforcement
        private DateTime twoYearFullInspection; //need to have better names for previous inspection dates and enforcement
        private string inspectionResults; //TODO connect inspection results with config file (NO, NO24, ENF, YES) 
        private DateTime[] datesofSOD; //TODO dates of SOD for substantiated complaints done since the last full inspection?
        private string enforcement; //TODO any enforcement for any inspection done since the last full inspection? (fines, stop placement, conditions, revocation, summary suspension)
        private bool failedFollowUp; //datatype for this?
        private string complaints; // failed follow-ups for current inspection or complaints
        private DateTime proposedDate;
        private float scheduleInterval;
        private float month15; //what 15 months is from the last full inspection
        private float month18; //what 18 months is from the last full inspection
        private int numberOfLicensors; //number of licensors needed.. for inspection I'm guessing?
        private int sampleSize; //sample size from last full inspection.. of inspectors?

        public string Name { get => name; set => name = value; }
        public string NameOfLicensee { get => nameOfLicensee; set => nameOfLicensee = value; }
        public char Unit { get => unit; set => unit = value; }
        public int LicenseNumber { get => licenseNumber; set => licenseNumber = value; }
        public string Address { get => address; set => address = value; }
        public int ZipCode { get => zipCode; set => zipCode = value; }
        public int NumberOfBeds { get => numberOfBeds; set => numberOfBeds = value; }
        public DateTime LastFullInspection { get => lastFullInspection; set => lastFullInspection = value; }
        public DateTime OneYearFullInspection { get => oneYearFullInspection; set => oneYearFullInspection = value; }
        public DateTime TwoYearFullInspection { get => twoYearFullInspection; set => twoYearFullInspection = value; }
        public string InspectionResults { get => inspectionResults; set => inspectionResults = value; }
        public DateTime[] DatesofSOD { get => datesofSOD; set => datesofSOD = value; }
        public string Enforcement { get => enforcement; set => enforcement = value; }
        public bool FailedFollowUp { get => failedFollowUp; set => failedFollowUp = value; }
        public string Complaints { get => complaints; set => complaints = value; }
        public DateTime ProposedDate { get => proposedDate; set => proposedDate = value; }
        public float ScheduleInterval { get => scheduleInterval; set => scheduleInterval = value; }
        public float Month15 { get => month15; }
        public float Month18 { get => month18; }
        public int NumberOfLicensors { get => numberOfLicensors; set => numberOfLicensors = value; }
        public int SampleSize { get => sampleSize; set => sampleSize = value; }
    }
}

/*
5. Facility    - Micki
    a. Standard format, in email from Lisa    
    b. Entering Data
*/
