using FlightTicketManager.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlightTicketManager.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Aircraft> Aircrafts { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
    }
}
