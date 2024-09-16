using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public class AircraftRepository : GenericRepository<Aircraft>, IAircraftRepository
    {
        private readonly DataContext _context;

        public AircraftRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Aircraft> GetAllActive()
        {
            return _context.Aircrafts.Where(a => a.IsActive);
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Aircrafts.Include(a => a.User);
        }

        public async Task<Aircraft> GetByIdWithTrackingAsync(int id)
        {
            return await _context.Aircrafts.FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
