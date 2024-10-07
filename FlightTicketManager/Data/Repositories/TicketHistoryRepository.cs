using System.Linq;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public class TicketHistoryRepository : GenericRepository<TicketHistory>, ITicketHistoryRepository
    {
        private readonly DataContext _context;

        public TicketHistoryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<TicketHistory> GetByUserId(string userEmail)
        {
            return _context.TicketsHistory
                           .Where(t => t.TicketBuyer == userEmail)
                           .AsQueryable();
        }
    }
}
