using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly DataContext _context;

        public FlightRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        public IQueryable GetAllWithUsersAircraftsAndCities()
        {
            return _context.Flights
                .Include(f => f.User)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .OrderBy(f => f.DepartureDateTime);
        }

        public async Task<Flight> GetByIdWithUsersAircraftsAndCitiesAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.User)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
