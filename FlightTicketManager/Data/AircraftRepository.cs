using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data
{
    public class AircraftRepository : GenericRepository<Aircraft>, IAircraftRepository
    {
        public AircraftRepository(DataContext context) : base(context)
        {
            
        }
    }
}
