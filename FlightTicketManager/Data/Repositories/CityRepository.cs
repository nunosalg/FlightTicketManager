using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
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
