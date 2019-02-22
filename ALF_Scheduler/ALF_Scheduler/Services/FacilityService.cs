using System.Collections.Generic;
using System.Linq;
using ALF_Scheduler.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ALF_Scheduler.Services
{
    public class FacilityService
    {
        public FacilityService(ApplicationDbContext context)
        {
            DbContext = context;
        }

        private ApplicationDbContext DbContext { get; }

        public Facility AddOrUpdateFacility(Facility facility)
        {
            if (facility.Id == default(int))
                DbContext.Facilities.Add(facility);
            else
                DbContext.Facilities.Update(facility);

            DbContext.SaveChanges();
            return facility;
        }

        public void RemoveInspection(Facility facility)
        {
            DbContext.Facilities.Remove(facility);
            DbContext.SaveChanges();
        }

        public ICollection<Facility> AddFacilities(ICollection<Facility> facilities)
        {
            DbContext.AddRange(facilities);
            DbContext.SaveChanges();
            return facilities;
        }

        public Facility Find(int id)
        {
            return DbContext.Facilities
                .SingleOrDefault(facility => facility.Id == id);
        }

        public List<Facility> FetchAll()
        {
            var facilityTask = DbContext.Facilities.ToListAsync();
            facilityTask.Wait();
            return facilityTask.Result;
        }

        public Inspection GetMostRecentInspection(int facilityID)
        {
            return DbContext.Inspections.Where(inspection => inspection.FacilityID == facilityID)
                .OrderBy(inspection => inspection.InspectionDate)
                .First();
        }

        public Inspection GetNthPreviousInspection(int facilityID, int n)
        {
            return DbContext.Inspections.Where(inspection => inspection.FacilityID == facilityID)
                .OrderBy(inspection => inspection.InspectionDate)
                .Skip(n - 1).First();
        }
    }
}