using Microsoft.EntityFrameworkCore;

namespace IncidentProject.Models
{
    public class IncidentDbContext : DbContext
    {
        public IncidentDbContext(DbContextOptions<IncidentDbContext> options)
            : base(options)
        {
        }
        public DbSet<CustomerModel> IncidentForm { get; set; }
        public DbSet<UserRequest> login { get; set; }
    }
}
