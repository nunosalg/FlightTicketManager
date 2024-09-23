using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly DataContext _context;

        public TicketRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Ticket> GetByIdWithFlightDetailsAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Flight)
                .ThenInclude(f => f.Origin)
                .Include(t => t.Flight)
                .ThenInclude(f => f.Destination)
                .Include(t => t.TicketBuyer)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public IQueryable<Ticket> GetByPassenger(string passengerId)
        {
            return _context.Tickets
                .Include(t => t.Flight)
                .ThenInclude(f => f.Origin)
                .Include(t => t.Flight)
                .ThenInclude(f => f.Destination)
                .Include(t => t.Flight)
                .ThenInclude(t => t.Aircraft)
                .Where(t => t.TicketBuyer.Id == passengerId);
        }

        public async Task<bool> PassengerAlreadyHasTicketInFlight(int flightId, string passengerId)
        {
            return await _context.Tickets
                .AnyAsync(t => t.Flight.Id == flightId && t.PassengerId == passengerId);
        }
    }
}
