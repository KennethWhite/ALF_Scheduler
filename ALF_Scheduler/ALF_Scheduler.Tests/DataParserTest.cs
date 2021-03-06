//// <copyright file="DataParserTest.cs">Copyright ©  2018</copyright>
//using System;
//using ALF_Scheduler;
//using Microsoft.Pex.Framework;
//using Microsoft.Pex.Framework.Validation;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace ALF_Scheduler.Tests
//{
//    /// <summary>This class contains parameterized unit tests for DataParser</summary>
//    [PexClass(typeof(DataParser))]
//    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
//    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
//    [TestClass]
//    public partial class DataParserTest {

//        DataParser dp;
//        Facility facility;

//        [TestInitialize]
//        public void init() {
//            facility = new Facility();
//            dp = new DataParser(facility);
//        }

//        [TestMethod]
//        public void CreateName_Success() {
//            dp.Name("facility name");
//            Assert.AreEqual("facility name", facility.FacilityName);
//        }

//        [TestMethod]
//        public void CreateLicensee_Success() {
//            dp.Licensee("name, licensee");
//            Assert.AreEqual("name, licensee", facility.NameOfLicensee);
//        }

//        [TestMethod]
//        public void CreateLicenseeFirstName_Success() {
//            dp.Licensee("name, licensee");
//            Assert.AreEqual("licensee", facility.LicenseeFirstName);
//        }

//        [TestMethod]
//        public void CreateLicenseeLastName_Success() {
//            dp.Licensee("name, licensee");
//            Assert.AreEqual("name", facility.LicenseeLastName);
//        }

//        [TestMethod]
//        public void CreateLicenseNumber_Success() {
//            dp.LicenseNumber("752103");
//            Assert.AreEqual(752103, facility.LicenseNumber);
//        }

//        [TestMethod]
//        public void CreateUnit_Success() {
//            dp.Unit("U");
//            Assert.AreEqual('U', facility.Unit);
//        }

//        [TestMethod]
//        public void CreateCity_Success() {
//            dp.City("Address");
//            Assert.AreEqual("Address", facility.City);
//        }

//        [TestMethod]
//        public void CreateZip_Success() {
//            dp.ZipCode("98001");
//            Assert.AreEqual(98001, facility.ZipCode);
//        }

//        [TestMethod]
//        public void CreateBeds_Success() {
//            dp.NumberOfBeds("24");
//            Assert.AreEqual(24, facility.NumberOfBeds);
//        }

//        [TestMethod]
//        public void CreateDateTime_Success() {
//            DateTime testDate = new DateTime(2018, 7, 22);
//            Assert.AreEqual(testDate, DataParser.CreateDateTime("7/22/18"));
//            Assert.AreEqual(testDate, DataParser.CreateDateTime("07/22/2018"));
//        }

//        [TestMethod]
//        public void CreateDateTime_Fail() {
//            DateTime testDate = new DateTime(2018, 7, 22);
//            Assert.AreNotEqual(testDate, DataParser.CreateDateTime("02/22/88"));
//            Assert.AreNotEqual(testDate, DataParser.CreateDateTime("02/22/2088"));
//        }

//        [TestMethod]
//        [ExpectedException(typeof(FormatException))]
//        public void CreateDateTime_Exception() {
//            DateTime testDate = new DateTime(2018, 7, 22);
//            Assert.AreEqual(testDate, DataParser.CreateDateTime("2/2/8"));
//        }

//    }
//}

