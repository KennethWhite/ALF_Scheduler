using ALF_Scheduler.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALF_Scheduler.Services
{
    public class InspectionService
    {
        private ApplicationDbContext DbContext { get; }

        public InspectionService(ApplicationDbContext context)
        {
            DbContext = context;
        }

        public Inspection AddOrUpdateInspection(Inspection inspection)
        {
            if (inspection.Id == default(int))
                DbContext.Inspections.Add(inspection);
            else
                DbContext.Inspections.Update(inspection);

            DbContext.SaveChanges();
            return inspection;
        }

        public void RemoveInspection(Inspection inspection)
        {
            DbContext.Inspections.Remove(inspection);    
            DbContext.SaveChanges();
        }

        public ICollection<Inspection> AddInspections(ICollection<Inspection> inspections)
        {
            DbContext.AddRange(inspections);
            DbContext.SaveChanges();
            return inspections;
        }

        public Inspection Find(int id)
        {
            return DbContext.Inspections
                .SingleOrDefault(Inspection => Inspection.Id == id);
        }

        public List<Inspection> FetchAll()
        {
            var inspectionTask = DbContext.Inspections.ToListAsync();
            inspectionTask.Wait();
            return inspectionTask.Result;
        }
    }
}
