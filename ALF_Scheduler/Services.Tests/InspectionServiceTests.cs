using System;
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
    public class InspectionServiceTests
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
        public void AddInspection_InspectionIsAddedIntoDatabase()
        {
            using (var dbContext = new ApplicationDbContext(Options))
            {
                var service = new InspectionService(dbContext);
                var inspectionFromDb = service.AddOrUpdateInspection(CreateInspection());
                Assert.AreNotEqual(default(int), inspectionFromDb.Id);
            }
        }

        [TestMethod]
        public void FindInspection_CreatedInspectionIsRetrievedFromDatabase()
        {
            using (var context = new ApplicationDbContext(Options))
            {
                var service = new InspectionService(context);
                var myInspection = CreateInspection();
                service.AddOrUpdateInspection(myInspection);
            }

            using (var context = new ApplicationDbContext(Options))
            {
                var service = new InspectionService(context);
                var fetchedInspection = service.Find(1);
                Assert.AreEqual(1, fetchedInspection.Id);
            }
        }

        [TestMethod]
        public void FindInspectionID2_CreatedInspectionIsRetrievedFromDatabase()
        {
            var inspectionList = CreateFiveInspections();
            using (var context = new ApplicationDbContext(Options))
            {
                new InspectionService(context).AddInspections(inspectionList);
            }

            using (var context = new ApplicationDbContext(Options))
            {
                var fetchedInspection = new InspectionService(context).Find(2);
                Assert.AreEqual(2, fetchedInspection.Id);
                Assert.AreEqual(inspectionList[1].InspectionDate, fetchedInspection.InspectionDate);
                Assert.AreEqual(inspectionList[1].Licensor, fetchedInspection.Licensor);
            }
        }

        [TestMethod]
        public void UpdateInspection_InspectionIsUpdatedInTheDatabase()
        {
            var myInspection = CreateInspection();
            using (var context = new ApplicationDbContext(Options))
            {
                new InspectionService(context).AddOrUpdateInspection(myInspection);
            }

            myInspection.Comments = "Updated Inspection Comment";
            myInspection.Licensor = "Bob Ross";


            using (var context = new ApplicationDbContext(Options))
            {
                new InspectionService(context).AddOrUpdateInspection(myInspection);
            }

            using (var context = new ApplicationDbContext(Options))
            {
                var fetchedInspection = new InspectionService(context).Find(1);
                Assert.AreEqual(1, fetchedInspection.Id);
                Assert.AreEqual("Updated Inspection Comment", fetchedInspection.Comments);
                Assert.AreEqual("Bob Ross", fetchedInspection.Licensor);
            }
        }


        [TestMethod]
        public void FetchInspections_CreatedInspectionsAreRetrievedFromDatabase()
        {
            using (var context = new ApplicationDbContext(Options))
            {
                new InspectionService(context).AddInspections(CreateFiveInspections());
            }

            using (var context = new ApplicationDbContext(Options))
            {
                var initialInspections = CreateFiveInspections();
                var fetchedInspections = new InspectionService(context).FetchAll();
                for (var i = 0; i < fetchedInspections.Count; i++)
                {
                    var inspectionToAdd = initialInspections[i];
                    var inspectionFetched = fetchedInspections[i];
                    Assert.AreNotEqual(default(int), inspectionFetched.Id);
                    Assert.AreEqual(inspectionToAdd.InspectionDate, inspectionFetched.InspectionDate);
                    //Assert.AreEqual<string>(inspectionToAdd.FacilityInspected.FacilityName, 
                    //  inspectionFetched.FacilityInspected.FacilityName);
                }
            }
        }

        private static Inspection CreateInspection()
        {
            var toAdd = new Facility
            {
                FacilityName = "Test Facility",
                City = "Spokane",
                NumberOfBeds = 42
            };
            return new Inspection
            {
                FacilityID = toAdd.Id,
                Licensor = "Bob Ross",
                InspectionDate = new DateTime(2019, 1, 26)
            };
        }

        private static List<Inspection> CreateFiveInspections()
        {
            var inspections = new List<Inspection>();
            for (var i = 1; i < 6; i++)
                inspections.Add(new Inspection
                {
                    Licensor = $"Bob Ross the {i}th",
                    InspectionDate = new DateTime(2019, 1, 26)
                });
            return inspections;
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