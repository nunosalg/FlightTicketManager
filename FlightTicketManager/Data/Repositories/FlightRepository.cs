using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;
using System.Collections.Generic;
using System;

namespace FlightTicketManager.Data.Repositories
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly DataContext _context;

        public FlightRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAvailableWithAircraftsAndCities()
        {
            return _context.Flights
                .Where(f => f.DepartureDateTime >= DateTime.UtcNow)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .OrderBy(f => f.DepartureDateTime);
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

        public async Task<IEnumerable<Flight>> GetFlightsByCriteriaAsync(int? originId, int? destinationId, DateTime? departureDate)
        {
            var query = _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .AsQueryable();

            if (originId.HasValue)
            {
                query = query.Where(f => f.Origin.Id == originId.Value);
            }

            if (destinationId.HasValue)
            {
                query = query.Where(f => f.Destination.Id == destinationId.Value);
            }

            if (departureDate.HasValue)
            {
                query = query.Where(f => f.DepartureDateTime.Date == departureDate.Value.Date);
            }

            return await query.ToListAsync();
        }
    }
}
