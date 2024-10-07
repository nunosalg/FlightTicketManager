using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<City> GetByIdWithTrackingAsync(int id)
        {
            return await _context.Cities.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> CheckIfCityExistsByName(string name)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == name);

            if (city == null)
                return false;
            return true;
        }
    }
}
