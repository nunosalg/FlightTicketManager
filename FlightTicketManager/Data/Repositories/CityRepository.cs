using FlightTicketManager.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlightTicketManager.Data.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        private readonly DataContext _context;

        public CityRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<City> GetByIdWithTrackingAsync(int id)
        {
            return await _context.Cities.FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
