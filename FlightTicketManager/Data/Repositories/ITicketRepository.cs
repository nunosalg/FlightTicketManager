using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket> GetByIdWithFlightDetailsAsync(int id);

        Task<bool> PassengerAlreadyHasTicketInFlight(int flightId, string passengerId);

        IQueryable<Ticket> GetTicketsByUserEmail(string userEmail);

        IQueryable GetTicketsHistoryByUser(string userId);

        Task<bool> HasTicketsByUserAsync(string userId);

        Task<List<Ticket>> GetTicketsByFlightIdAsync(int flightId);
    }
}
