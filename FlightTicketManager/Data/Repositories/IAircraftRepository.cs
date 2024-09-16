using System.Linq;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface IAircraftRepository : IGenericRepository<Aircraft>
    {
        public IQueryable GetAllWithUsers();

        public Task<Aircraft> GetByIdWithTrackingAsync(int id);

        public IQueryable<Aircraft> GetAllActive();
    }
}
