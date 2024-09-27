using System.Linq;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface IAircraftRepository : IGenericRepository<Aircraft>
    {
        IQueryable GetAllWithUsers();

        Task<Aircraft> GetByIdWithTrackingAsync(int id);

        IQueryable<Aircraft> GetAllActive();
    }
}
