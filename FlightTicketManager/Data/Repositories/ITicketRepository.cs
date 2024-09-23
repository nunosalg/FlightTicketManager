using System.Linq;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket> GetByIdWithFlightDetailsAsync(int id);

        Task<bool> PassengerAlreadyHasTicketInFlight(int flightId, string passengerId);

        IQueryable<Ticket> GetByPassenger(string passengerId);
    }
}
