using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        public IQueryable GetAllWithUsersAircraftsAndCities();

        public IQueryable GetAvailableWithAircraftsAndCities();

        Task<Flight> GetByIdWithUsersAircraftsAndCitiesAsync(int id);

        Task<IEnumerable<Flight>> GetFlightsByCriteriaAsync(int? originId, int? destinationId, DateTime? departureDate);
    }
}
