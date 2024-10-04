using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Services
{
    public interface IHistoryService
    {
        Task SaveFlightHistoryAsync(Flight flight, string flightStatus, string ticketStatus);

        Task SaveTicketHistoryAsync(Ticket ticket, string ticketStatus);
    }
}
