using ALF_Scheduler;
using Microsoft.EntityFrameworkCore;

namespace ALF_Scheduler.Domain.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
    }
}