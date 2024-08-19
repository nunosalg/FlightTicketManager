using System.Linq;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data
{
    public interface IAircraftRepository : IGenericRepository<Aircraft>
    {
        public IQueryable GetAllWithUsers();
    }
}
