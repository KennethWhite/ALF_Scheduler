using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALF_Scheduler
{
    class DataParser
    {
        // the facility name, licensee, license number,unit, address?city?, zipcode, number of beds, previous full inspection, most recent full inspection, inspection results 
        // proposed scheduling date, number of days and months between the last full inspection and the proposed scheduling date, what 15 months is from the last full inspection, 
        // what 18 months is from the last full inspection, number of licensors needed, sample size from last full inspection

        /*
        DatesOfSOD // dates of SOD (Statement of Deficiencies report) for substantiated complaints done since the last full inspection?
        EnforcementNotes  // any enforcement for any inspection done since the last full inspection? (fines, stop placement, conditions, revocation, summary suspension)
        FailedFollowUp
        Complaints  // failed follow-ups for current inspection or complaints*/

        private string[] _testExcelColumns = { "facility name", "licensee name", "U", "752103", "98001", "City", "08/14/2016", "02/23/2018", "03/23/2019", "07/25/2019", "08/25/2019", "Miranda", "Yes" };
        Facility facility = new Facility();

        private void Name() {
            facility.Name = _testExcelColumns[0];
        }

        private void Licensee() {
            string[] licensee = _testExcelColumns[1].Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            facility.LicenseeLastName = licensee[0];
            facility.LicenseeFirstName = licensee[1];
        }

        private void Unit() {
            facility.Unit = Convert.ToChar(_testExcelColumns[2]);
        }

        private void LicenseNumber() {
            facility.LicenseNumber = Convert.ToInt32(_testExcelColumns[3]);
        }

        private void ZipCode() {
            facility.ZipCode = Convert.ToInt32(_testExcelColumns[4]);
        }

        private void City() {
            //facility.City = _testExcelColumns[5];
        }

        private DateTime CreateDateTime(string date) {
            string[] dateArray = date.Split('/');
            int month = Convert.ToInt16(dateArray[0]);
            int day = Convert.ToInt16(dateArray[1]);
            int year = Convert.ToInt16(dateArray[2]);
            return new DateTime(year, month, day);
        }

        private void PreviousInspection() {
            facility.OneYearFullInspection = CreateDateTime(_testExcelColumns[6]);
        }

        private void LastInspection() {
            facility.LastFullInspection = CreateDateTime(_testExcelColumns[7]);
        }

        private void ProposedDate() {
            facility.ProposedDate = CreateDateTime(_testExcelColumns[7]);
        }

        private void NumberOfBeds() {
            facility.NumberOfBeds = Convert.ToInt16(_testExcelColumns[8]);
        }
    }
}
