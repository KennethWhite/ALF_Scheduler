using System.Collections.Generic;
using ALF_Scheduler;
using ALF_Scheduler.Domain.Models;
using ALF_Scheduler.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Services.Tests
{
    [TestClass]
    public class FacilityServiceTests
    {
        private SqliteConnection SqliteConnection { get; set; }
        private DbContextOptions<ApplicationDbContext> Options { get; set; }

        [TestInitialize]
        public void OpenConnection()
        {
            SqliteConnection = new SqliteConnection("DataSource=:memory:");
            SqliteConnection.Open();

            Options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(SqliteConnection)
                .UseLoggerFactory(GetLoggerFactory())
                .EnableSensitiveDataLogging()
                .Options;

            using (var context = new ApplicationDbContext(Options))
            {
                context.Database.EnsureCreated();
            }
        }

        [TestCleanup]
        public void CloseConnection()
        {
            SqliteConnection.Close();
        }

        [TestMethod]
        public void AddFacility_FacilityIsAddedIntoDatabase()
        {
            using (var dbContext = new ApplicationDbContext(Options))
            {
                var service = new FacilityService(dbContext);
                var facilityFromDb = service.AddOrUpdateFacility(CreateFacility());
                Assert.AreNotEqual(0, facilityFromDb.Id);
            }
        }

        [TestMethod]
        public void FindFacility_CreatedFacilityIsRetrievedFromDatabase()
        {
            using (var context = new ApplicationDbContext(Options))
            {
                var service = new FacilityService(context);
                var myFacility = CreateFacility();
                service.AddOrUpdateFacility(myFacility);
            }

            using (var context = new ApplicationDbContext(Options))
            {
                var service = new FacilityService(context);
                var fetchedFacility = service.Find(1);
                Assert.AreEqual(1, fetchedFacility.Id);
            }
        }

        [TestMethod]
        public void FindFacilityID2_CreatedFacilityIsRetrievedFromDatabase()
        {
            var facilityList = CreateFiveFacilities();
            using (var context = new ApplicationDbContext(Options))
            {
                new FacilityService(context).AddFacilities(facilityList);
            }

            using (var context = new ApplicationDbContext(Options))
            {
                var fetchedFacility = new FacilityService(context).Find(2);
                Assert.AreEqual(2, fetchedFacility.Id);
                Assert.AreEqual(facilityList[1].FacilityName, fetchedFacility.FacilityName);
                Assert.AreEqual(facilityList[1].InspectionResult, fetchedFacility.InspectionResult);
                //Assert.AreEqual(facilityList[1].MostRecentFullInspection().InspectionDate, 
                //    fetchedFacility.MostRecentFullInspection().InspectionDate);
            }
        }

        [TestMethod]
        public void UpdateFacility_FacilityIsUpdatedInTheDatabase()
        {
            var myFacility = CreateFacility();
            using (var context = new ApplicationDbContext(Options))
            {
                new FacilityService(context).AddOrUpdateFacility(myFacility);
            }

            myFacility.FacilityName = "Updated Facility Name";
            myFacility.NumberOfBeds = 1001;

            using (var context = new ApplicationDbContext(Options))
            {
                new FacilityService(context).AddOrUpdateFacility(myFacility);
            }

            using (var context = new ApplicationDbContext(Options))
            {
                var fetchedFacility = new FacilityService(context).Find(1);
                Assert.AreEqual(1, fetchedFacility.Id);
                Assert.AreEqual("Updated Facility Name", fetchedFacility.FacilityName);
                Assert.AreEqual(1001, fetchedFacility.NumberOfBeds);
            }
        }


        [TestMethod]
        public void FetchFacilities_CreatedFacilitiesAreRetrievedFromDatabase()
        {
            using (var context = new ApplicationDbContext(Options))
            {
                new FacilityService(context).AddFacilities(CreateFiveFacilities());
            }

            using (var context = new ApplicationDbContext(Options))
            {
                var initialFacilitys = CreateFiveFacilities();
                var fetchedFacilitys = new FacilityService(context).FetchAll();
                for (var i = 0; i < fetchedFacilitys.Count; i++)
                {
                    var facilityToAdd = initialFacilitys[i];
                    var facilityFetched = fetchedFacilitys[i];
                    Assert.AreNotEqual(default(int), facilityFetched.Id);
                    Assert.AreEqual(facilityToAdd.FacilityName, facilityFetched.FacilityName);
                }
            }
        }

        private static Facility CreateFacility()
        {
            return new Facility
            {
                FacilityName = "Test Facility",
                InspectionResult = "PASS",
                NumberOfBeds = 42,
                City = "Spokane"
                //PreviousFullInspection = new Inspection()
                //{
                //    InspectionDate = new System.DateTime(2019, 1, 25),
                //    Licensor = "Bob Ross"
                //}
            };
        }

        private static List<Facility> CreateFiveFacilities()
        {
            var facilities = new List<Facility>();
            for (var i = 1; i < 6; i++)
                facilities.Add(new Facility
                {
                    FacilityName = $"Test Facility {i}",
                    InspectionResult = "PASS",
                    NumberOfBeds = 42,
                    City = "Spokane"
                    //MostRecentFullInspection = new Inspection()
                    //{
                    //    InspectionDate = new System.DateTime(2019, 1, 25),
                    //    Licensor = "Bob Ross"
                    //}
                });
            return facilities;
        }

        private static ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConsole()
                    .AddFilter(DbLoggerCategory.Database.Command.Name,
                        LogLevel.Information);
            });
            return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
        }
    }
}