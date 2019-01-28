//using ALF_Scheduler.Domain.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace ALF_Scheduler {

//    /// <summary>
//    /// This <c>DataParser</c> class verifies and parses the incoming data to be 
//    /// sent to the <see cref="Facility"/> object.
//    /// </summary>
//    /// <remarks>Each method takes in a string (or strings) as parameters.</remarks>
//    class DataParser
//    {
//        Facility facility;

//        /// <summary>
//        /// The DataParser constructor for a new facility.
//        /// </summary>
//        public DataParser() {
//            facility = new Facility();
//        }

//        /// <summary>
//        /// The DataParser constructor for a specific facility.
//        /// </summary>
//        /// <param name="fac">The facility object to be used.</param>
//        public DataParser(Facility fac) {
//            facility = fac;
//        }

//        /// <summary>
//        /// This method creates the facility <paramref name="name"/>.
//        /// </summary>
//        /// <param name="name">The facility's name as a string.</param>
//        public void Name(string name) {
//            facility.FacilityName = name;
//        }

//        /// <summary>
//        /// This method will split the <paramref name="licensee"/>'s name into first and last 
//        /// and place them into their properties.
//        /// </summary>
//        /// <param name="licensee">The licensee's full name as a string separated by a comma.</param>
//        public void Licensee(string licensee) {
//            string[] firstLast = licensee.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            
//            facility.LicenseeLastName = firstLast[0];
//            facility.LicenseeFirstName = firstLast[1];
//        }

//        /// <summary>
//        /// This method place the licensee's <paramref name="first"/> and 
//        /// <paramref name="last"/> names into their properties.
//        /// </summary>
//        /// <param name="first">The licensee's first name as a string.</param>
//        /// <param name="last">The licensee's last name as a string.</param>
//        public void Licensee(string first, string last) {
//            facility.LicenseeFirstName = first;
//            facility.LicenseeLastName = last;
//        }
        
//        /// <summary>
//        /// This method stores the facility <paramref name="unit"/> in the facility object.
//        /// </summary>
//        /// <param name="unit">The facility's unit as a string.</param>
//        public void Unit(string unit) {
//            try {
//                facility.Unit = unit;
//            } catch (Exception e) {
//                Console.WriteLine("{0} exception in unit", e);
//            }
//        }

//        /// <summary>
//        /// This method creates the facility's license <paramref name="number"/>.
//        /// </summary>
//        /// <param name="number">The facility's license number as a string.</param>
//        /// <exception cref="InvalidCastException">InvalidCastException thrown if the string cannot
//        /// be converted to an int.</exception>
//        public void LicenseNumber(string number) {
//            try {
//                facility.LicenseNumber = Convert.ToInt32(number);
//            } catch (InvalidCastException e) {
//                Console.WriteLine("License number string was unable to be converted to an int. {0]", e);
//            } catch (Exception e) {
//                Console.WriteLine("{0} exception in license number", e);
//            }
//        }

//        /// <summary>
//        /// This method creates the facility <paramref name="zip"/>code.
//        /// </summary>
//        /// <param name="zip">The facility's zipcode as a string.</param>
//        /// <exception cref="InvalidCastException">InvalidCastException thrown if the string cannot
//        /// be converted to an int.</exception>
//        public void ZipCode(string zip) {
//            try {
//                facility.ZipCode = Convert.ToInt16(zip);
//            } catch (InvalidCastException e) {
//                Console.WriteLine("Zipcode string was unable to be converted to an int {0]", e);
//            } catch (Exception e) {
//                Console.WriteLine("{0} exception in zipcode", e);
//            }
//        }

//        /// <summary>
//        /// This method creates the number of <paramref name="beds"/> in the facility.
//        /// </summary>
//        /// <param name="beds">The number of beds in the facility as a string.</param>
//        /// <exception cref="InvalidCastException">InvalidCastException thrown if the string cannot 
//        /// be converted to an int.</exception>
//        public void NumberOfBeds(string beds) {
//            try {
//                facility.NumberOfBeds = Convert.ToInt16(beds);
//            } catch (InvalidCastException e) {
//                Console.WriteLine("String of beds was unable to be converted to an int {0]", e);
//            } catch (Exception e) {
//                Console.WriteLine("{0} exception in number of beds", e);
//            }
//        }

//        /// <summary>
//        /// This method creates the facility <paramref name="city"/>.
//        /// </summary>
//        /// <param name="city">The city the facility resides in as a string.</param>
//        public void City(string city) {
//            facility.City = city;
//        }

//        /// <summary>
//        /// This method will check the incoming string against the regex
//        /// pattern for dates, then create a DateTime object from the <paramref name="date"/>.
//        /// </summary>
//        /// <remarks>
//        /// <para>The date can be in the form of MM/DD/YY or MM/DD/YYYY.</para>
//        /// <para>The month can be with or without a leading zero (03 or just 3 is fine).</para>
//        /// </remarks>
//        /// <param name="date">The date to be created as a string.</param>
//        /// <returns>DateTime object</returns>
//        /// <exception cref="FormatException">Throws FormatException if date is not in the correct format.</exception>
//        public static DateTime CreateDateTime(string date) {
//            Regex rx = new Regex("([0-9]|[0-9]{2})/([0-9]{2})/([0-9]{4}|[0-9]{2})");
//            if (rx.IsMatch(date)) {
//                return DateTime.Parse(date);
//            } else {
//                throw new FormatException("date does not match regex format");
//            }
//        }

//        /// <summary>
//        /// This method creates the facility's most recent inspection <paramref name="date"/> 
//        /// by using the CreateDateTime method here: <see cref="DataParser.CreateDateTime(string date)"/>.
//        /// </summary>
//        /// <param name="date">The facility's most recent inspection date as a string.</param>
//        public void MostRecentInspection(string date) {
//            facility.MostRecentFullInspection = new Inspection() {InspectionDate = CreateDateTime(date) };
//        }

//        /// <summary>
//        /// This method creates the facility's one year inspection <paramref name="date"/> 
//        /// by using the CreateDateTime method here: <see cref="DataParser.CreateDateTime(string date)"/>.
//        /// </summary>
//        /// <param name="date">The facility's one year inspection date as a string.</param>
//        public void OneYearInspection(string date) {
//            facility.PreviousFullInspection = new Inspection() { InspectionDate = CreateDateTime(date) };
//        }

//        /// <summary>
//        /// This method creates the facility's two year inspection <paramref name="date"/> 
//        /// by using the CreateDateTime method here: <see cref="DataParser.CreateDateTime(string date)"/>.
//        /// </summary>
//        /// <param name="date">The facility's two year inspection date as a string.</param>
//        public void TwoYearInspection(string date) {
//            facility.TwoYearFullInspection = new Inspection() { InspectionDate = CreateDateTime(date) };
//        }

//        //TODO connect to config file
//        /// <summary>
//        /// This method sets the inspection <paramref name="result"/> from the result code config file.
//        /// </summary>
//        /// <param name="result">The inspection result as a string.</param>
//        public void InspectionResult(string result) {
//            if (true) { 
//                facility.InspectionResult = result;
//            } else {
//                // create new code
//            }
//        }

//        //TODO dates of SOD (Statement of Deficiencies report)
//        /// <summary>
//        /// This method creates a DateTime array for <paramref name="dates"/> of substantiated complaints done 
//        /// since the last full inspection.
//        /// </summary>
//        /// <remarks>
//        /// This method splits the incoming string into an array, creates DateTime objects from each date string in the array, 
//        /// and sends the new DateTime array to the facility object.
//        /// </remarks>
//        /// <param name="dates">The dates as a string.</param>
//        public void DatesOfSOD(string dates) {
//            try {
//                string[] dateString = dates.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
//                DateTime[] SODs = new DateTime[dateString.Length];
//                for (int x = 0; x < dateString.Length; x++) {
//                    SODs[x] = CreateDateTime(dateString[x]);
//                }
//                facility.DatesOfSOD = SODs;
//            } catch (Exception e) {
//                Console.WriteLine("{0} exception in dates of SOD", e);
//            }
//        }

//        //TODO Complaints
//        /// <summary>
//        /// This method creates a string for <paramref name="complaints"/> made since last full inspection.
//        /// </summary>
//        /// <param name="complaints">The complaints as a string.</param>
//        public void Complaints(string complaints) {

//        }

//        //TODO Enforcement Notes
//        /// <summary>
//        /// This method creates a string for any enforcement <paramref name="notes"/> for any inspection 
//        /// done since the last full inspection (fines, stop placement, conditions, revocation, summary suspension).
//        /// </summary>
//        /// <param name="notes">The enforcement notes as a string.</param>
//        public void EnforcementNotes(string notes) {
//            facility.EnforcementNotes = notes;
//        }

//        //TODO determine best way to do Failed Follow Up
//        /// <summary>
//        /// This method creates a DateTime array of <paramref name="dates"/> from failed follow ups from the most recent inspection 
//        /// or complaint based follow ups.
//        /// </summary>
//        /// <remarks>
//        /// This method splits the incoming string into an array, creates DateTime objects from each date string in the array, 
//        /// and sends the new DateTime array to the facility object.
//        /// </remarks>
//        /// <param name="dates">The failed follow up dates as a string.</param>
//        public void FailedFollowUp(string dates) {
//            try {
//                string[] dateString = dates.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
//                DateTime[] followUpDates = new DateTime[dateString.Length];
//                for (int x = 0; x < dateString.Length; x++) {
//                    followUpDates[x] = CreateDateTime(dateString[x]);
//                }
//                facility.FailedFollowUp = followUpDates;
//            } catch (Exception e) {
//                Console.WriteLine("{0} exception in failed follow up", e);
//            }
//        }

//        /// <summary>
//        /// This method creates the facility's proposed inspection <paramref name="date"/> by using the CreateDateTime 
//        /// method here: <see cref="DataParser.CreateDateTime(string date)"/>.
//        /// </summary>
//        /// <param name="date">The facility's proposed inspection date as a string.</param>
//        public void ProposedDate(string date) {
//            facility.ProposedDate = CreateDateTime(date);
//        }

//        /// <summary>
//        /// This method creates the <paramref name="number"/> of licensors needed for inspections based on bed count.
//        /// </summary>
//        /// <param name="number">The number of beds in the facility as a string.</param>
//        /// <exception cref="InvalidCastException">InvalidCastException thrown if the string cannot be converted to an int.</exception>
//        public void NumberOfLicensors(string number) {
//            try {
//                facility.NumberOfLicensors = Convert.ToInt16(number);
//            } catch (InvalidCastException e) {
//                Console.WriteLine("String of licensors was unable to be converted to an int {0]", e);
//            } catch (Exception e) {
//                Console.WriteLine("{0} exception in number of licensors", e);
//            }
//        }

//        /// <summary>
//        /// This method creates the <paramref name="list"/> of licensors on the last inspection.
//        /// </summary>
//        /// <remarks>The first licensor in the array is marked with an '*' to denote them as the team leader.</remarks>
//        /// <param name="size">The sample size of inspectors as a string.</param>
//        public void LicensorList(string list) {
//            try {
//                string[] listArray = list.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
//                FormattableString formattableString = $"{listArray[0]}*";
//                listArray[0] = formattableString.ToString();
//                facility.LicensorList = listArray;
//            } catch (Exception e) {
//                Console.WriteLine("{0} exception in licensor list", e);
//            }
//        }

//        /// <summary>
//        /// This method creates the facility's special <paramref name="info"/> column for items such as memory care and mental health.
//        /// </summary>
//        /// <param name="info">The facility's special information as a string.</param>
//        public void SpecialInfo(string info) {
//            facility.SpecialInfo = info;
//        }
//    }
//}
