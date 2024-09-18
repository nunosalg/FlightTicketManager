using System.Linq;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        public IQueryable GetAllWithUsersAircraftsAndCities();

        Task<Flight> GetByIdWithUsersAircraftsAndCitiesAsync(int id);
    }
}
