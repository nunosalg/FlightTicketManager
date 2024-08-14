using FlightTicketManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightTicketManager.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Aircraft> Aircrafts { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
    }
}
