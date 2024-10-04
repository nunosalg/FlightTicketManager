using System.Linq;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface ITicketHistoryRepository : IGenericRepository<TicketHistory>
    {
        IQueryable<TicketHistory> GetByUserId(string userEmail);
    }
}
