using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        private readonly DataContext _context;

        public CityRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
