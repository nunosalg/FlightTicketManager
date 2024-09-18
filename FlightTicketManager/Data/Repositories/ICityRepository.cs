using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface ICityRepository : IGenericRepository<City>
    {
        Task<City> GetByIdWithTrackingAsync(int id);
    }
}
