using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;
using System.Collections.Generic;

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

        public IQueryable<Ticket> GetTicketsByUserEmail(string userEmail)
        {
            return _context.Tickets
                .Include(t => t.TicketBuyer)
                .Include(t => t.Flight) 
                .ThenInclude(f => f.Origin)
                .Include(t => t.Flight)
                .ThenInclude(f => f.Destination) 
                .Include(t => t.Flight)
                .ThenInclude(f => f.Aircraft) 
                .Where(t => t.TicketBuyer.Email == userEmail && t.Flight.DepartureDateTime > DateTime.Now);


        }

        public IQueryable GetTicketsHistoryByUser(string userId)
        {
            return _context.Tickets
                .Include(t => t.Flight)
                .ThenInclude(f => f.Origin)
                .Include(t => t.Flight)
                .ThenInclude(f => f.Destination)
                .Include(t => t.Flight)
                .ThenInclude(t => t.Aircraft)
                .Where(t => t.TicketBuyer.Id == userId && t.Flight.DepartureDateTime < DateTime.Now);
        }

        public async Task<bool> PassengerAlreadyHasTicketInFlight(int flightId, string passengerId)
        {
            return await _context.Tickets
                .AnyAsync(t => t.Flight.Id == flightId && t.PassengerId == passengerId);
        }

        public async Task<bool> HasTicketsByUserAsync(string userId)
        {
            return await _context.Tickets.AnyAsync(f => f.TicketBuyer.Id == userId);
        }

        public async Task<List<Ticket>> GetTicketsByFlightIdAsync(int flightId)
        {
            return await _context.Tickets
                .Where(t => t.Flight.Id == flightId)
                .Include(t => t.TicketBuyer)
                .Include(t => t.Flight)
                .ToListAsync();
        }
    }
}
