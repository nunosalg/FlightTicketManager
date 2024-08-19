using System.Linq;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data
{
    public class AircraftRepository : GenericRepository<Aircraft>, IAircraftRepository
    {
        private readonly DataContext _context;

        public AircraftRepository(DataContext context) : base(context)
        {
            _context = context;
        }


        public IQueryable GetAllWithUsers()
        {
            return _context.Aircrafts.Include(a => a.User);
        }
    }
}
