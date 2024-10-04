using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public class FlightHistoryRepository : GenericRepository<FlightHistory>, IFlightHistoryRepository
    {
        public FlightHistoryRepository(DataContext context) : base(context)
        {
        }
    }
}
