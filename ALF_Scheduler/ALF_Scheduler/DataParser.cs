using System;
using System.Text.RegularExpressions;
using ALF_Scheduler.Models;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using ALF_Scheduler.Utilities;
using Excel_Import_Export;

namespace ALF_Scheduler
{
    /// <summary>
    ///     This <c>DataParser</c> class verifies and parses the incoming data to be
    ///     sent to the <see cref="Facility" /> object.
    /// </summary>
    /// <remarks>Each method takes in a string (or strings) as parameters.</remarks>
    public static class DataParser
    {
        private static Facility _facility;
        private static Range _row;

        /// <summary>
        ///     This method will construct and return a facility object after parsing all the information from the specified
        ///     Excel.Range object representing the row.
        /// </summary>
        public static Facility ParseFacility(Range excelRow)
        {
            _facility = new Facility();
            _row = excelRow;
            return UpdateFacility(_facility, _row);
        }

        /// <summary>
        ///     This method will update an existing facility object after parsing all the information from the specified
        ///     Excel.Range object representing the row.
        /// </summary>
        /// <param name="fac">The facility object to be used.</param>
        /// <param name="excelRow">The Excel.Range object representing the row from the Excel Workbook to parse.</param>
        public static Facility UpdateFacility(Facility fac, Range excelRow)
        {
            _facility = fac;
            _row = excelRow;
            _facility.FacilityName = (string) (_row.Cells[1,1] as Range).Value2;
            SetLicensee((string)(_row.Cells[1, 2] as Range).Value2);
            _facility.Unit = (string)(_row.Cells[1, 3] as Range).Value2;
            _facility.LicenseNumber = ((double)(_row.Cells[1, 4] as Range).Value2).ToString();
            _facility.ZipCode = ((double)(_row.Cells[1, 5] as Range).Value2).ToString();
            _facility.City = (string)(_row.Cells[1, 6] as Range).Value2;
            _facility.AddInspection(CreateInspection((string)(_row.Cells[1, 7] as Range).Text));
            _facility.EnforcementNotes = (string)(_row.Cells[1, 15] as Range).Value2;
            _facility.AddInspection(CreateInspection((string)(_row.Cells[1, 8] as Range).Text, null, _facility.EnforcementNotes));
            _facility.LicensorList = (string)(_row.Cells[1, 14] as Range).Value2;
            var code = (string)(_row.Cells[1, 15] as Range).Value2;
            _facility.InspectionResult = code.ToUpper();
            _facility.SpecialInfo = (string)(_row.Cells[1, 20] as Range).Value2;
            string beds = ((double)(_row.Cells[1, 19] as Range).Value2).ToString();
            SetNumberOfBeds(beds);
            _facility.Complaints = (string)(_row.Cells[1, 16] as Range).Value2;



            return _facility;
        }

        private static Inspection CreateInspection(string date, string licensor = "", string code = "")
        {
            var ret = new Inspection { InspectionDate = DateTime.FromOADate(double.Parse(date)) };
            if (!string.IsNullOrEmpty(code)) ret.Code = Code.getCodeByName(code);
            ret.Licensor = licensor;
            return ret;
        }

        /// <summary>
        ///     This method will split the <paramref name="licensee" />'s name into first and last
        ///     and place them into their properties. If there is only a single name the FirstName property will
        ///     not be set
        /// </summary>
        /// <param name="licensee">The licensee's full name as a string separated by a comma.</param>
        private static void SetLicensee(string licensee)
        {
            if (licensee.Contains(","))
            {
                var firstLast = licensee.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                _facility.LicenseeLastName = firstLast[0];
                _facility.LicenseeFirstName = firstLast[1];
            }
            else
            {
                _facility.LicenseeLastName = licensee;
            }
        }

        /// <summary>
        ///     This method parsed the number of <paramref name="beds" /> in the facility, or upon failure to parse defaults to
        ///     zero.
        /// </summary>
        /// <param name="beds">The number of beds in the facility as a string.</param>
        /// <returns>True if bed count was successfully parsed, false otherwise.</returns>
        public static bool SetNumberOfBeds(string beds)
        {
            var success = int.TryParse(beds, out var parsed);
            _facility.NumberOfBeds = success ? parsed : default(int);
            return success;
        }


        /// <summary>
        ///     This method will check the incoming string against the regex
        ///     pattern for dates, then create a DateTime object from the <paramref name="date" />.
        /// </summary>
        /// <remarks>
        ///     <para>The date can be in the form of MM/DD/YY or MM/DD/YYYY.</para>
        ///     <para>The month can be with or without a leading zero (03 or just 3 is fine).</para>
        /// </remarks>
        /// <param name="date">The date to be created as a string.</param>
        /// <returns>DateTime object</returns>
        /// <exception cref="FormatException">Throws FormatException if date is not in the correct format.</exception>
        public static DateTime CreateDateTime(string date)
        {
            var regex = new Regex(@"^((\d{1,2}(-)\d{1,2}(-)\d{4})|(\d{1,2}(\/)\d{1,2}(\/)\d{4}))$");
            if (regex.IsMatch(date))
                return DateTime.Parse(date);
            throw new FormatException("date does not match regex format");
        }
    }
}