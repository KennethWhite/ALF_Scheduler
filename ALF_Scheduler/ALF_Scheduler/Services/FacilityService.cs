using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALF_Scheduler.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ALF_Scheduler.Services
{
    public class FacilityService
    {
        private ApplicationDbContext DbContext { get; }

        public FacilityService(ApplicationDbContext context)
        {
            DbContext = context;
        }

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
            return DbContext.Facilities.Include(facility => facility.MostRecentFullInspection)
                .SingleOrDefault(facility => facility.Id == id);
        }

        public List<Facility> FetchAll()
        {
            var facilityTask = DbContext.Facilities.ToListAsync();
            facilityTask.Wait();
            return facilityTask.Result;
        }
    }

}
}
