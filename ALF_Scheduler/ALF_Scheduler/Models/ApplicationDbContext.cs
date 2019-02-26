using ALF_Scheduler.Models;
using Microsoft.EntityFrameworkCore;

namespace ALF_Scheduler.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<Code> Codes { get; set; }
    }
}