using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        IQueryable GetAllWithUsersAircraftsAndCities();

        IQueryable GetFlightsHistoryWithAircraftsAndCities();

        IQueryable GetAvailableWithAircraftsAndCities();

        IQueryable GetAvailableWithUsersAircraftsAndCities();

        Task<Flight> GetByIdWithUsersAircraftsAndCitiesAsync(int id);

        Task<IEnumerable<Flight>> GetFlightsByCriteriaAsync(int? originId, int? destinationId, DateTime? departureDate);

        Task<Flight> GetByIdWithTrackingAsync(int id);

        List<Flight> GetConflictingFlights(
            Aircraft selectedAircraft,
            DateTime selectedDate,
            string selectedOrigin,
            string selectedDestination,
            Flight currentFlight = null);

        List<Flight> GetSameDayFlights(Aircraft selectedAircraft, DateTime selectedDate, Flight currentFlight = null);
    }
}
