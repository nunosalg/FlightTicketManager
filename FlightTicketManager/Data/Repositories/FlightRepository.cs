using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data.Repositories
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly DataContext _context;

        public FlightRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAvailableWithAircraftsAndCities()
        {
            return _context.Flights
                .Where(f => f.DepartureDateTime >= DateTime.UtcNow)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .OrderBy(f => f.DepartureDateTime);
        }

        public IQueryable GetAvailableWithUsersAircraftsAndCities()
        {
            return _context.Flights
                .Where(f => f.DepartureDateTime >= DateTime.UtcNow)
                .Include(f => f.User)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .OrderBy(f => f.DepartureDateTime);
        }

        public IQueryable GetFlightsHistoryWithAircraftsAndCities()
        {
            return _context.Flights
                .Where(f => f.DepartureDateTime < DateTime.UtcNow)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .OrderBy(f => f.DepartureDateTime);
        }

        public IQueryable GetAllWithUsersAircraftsAndCities()
        {
            return _context.Flights
                .Include(f => f.User)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .OrderBy(f => f.DepartureDateTime);
        }

        public async Task<Flight> GetByIdWithUsersAircraftsAndCitiesAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.User)
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .Include(f => f.TicketsList)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Flight>> GetFlightsByCriteriaAsync(int? originId, int? destinationId, DateTime? departureDate)
        {
            var query = _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .AsQueryable();

            if (originId.HasValue)
            {
                query = query.Where(f => f.Origin.Id == originId.Value);
            }

            if (destinationId.HasValue)
            {
                query = query.Where(f => f.Destination.Id == destinationId.Value);
            }

            if (departureDate.HasValue)
            {
                query = query.Where(f => f.DepartureDateTime.Date == departureDate.Value.Date);
            }

            return await query.ToListAsync();
        }

        public async Task<Flight> GetByIdWithTrackingAsync(int id)
        {
            return await _context.Flights.FirstOrDefaultAsync(a => a.Id == id);
        }

        public List<Flight> GetConflictingFlights(
            Aircraft selectedAircraft, 
            DateTime selectedDate, 
            string selectedOrigin, 
            string selectedDestination, 
            Flight currentFlight = null)
        {
            var conflictingFlights = _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .Where(v => v.Aircraft.Id == selectedAircraft.Id &&
                            v.DepartureDateTime.Date == selectedDate.Date &&
                            ((selectedOrigin != v.Origin.Name && selectedOrigin != v.Destination.Name) ||
                             (selectedDestination != v.Origin.Name && selectedDestination != v.Destination.Name)))
                .ToList();

            if (currentFlight != null)
            {
                conflictingFlights.RemoveAll(f => f.Id == currentFlight.Id);
            }

            return conflictingFlights;
        }

        public List<Flight> GetSameDayFlights(Aircraft selectedAircraft, DateTime selectedDate, Flight currentFlight = null)
        {
            var sameDayFlights = _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Origin)
                .Include(f => f.Destination)
                .Where(v => v.Aircraft.Id == selectedAircraft.Id &&
                            v.DepartureDateTime.Date == selectedDate.Date)
                .ToList();

            if (currentFlight != null)
            {
                sameDayFlights.RemoveAll(f => f.Id == currentFlight.Id);
            }

            return sameDayFlights;
        }

        public async Task<bool> HasFlightsWithAircraftAsync(int aircraftId)
        {
            return await _context.Flights.AnyAsync(f => f.Aircraft.Id == aircraftId);
        }

        public async Task<bool> HasFlightsWithCityAsync(int cityId)
        {
            return await _context.Flights.AnyAsync(f => f.Origin.Id == cityId || f.Destination.Id == cityId);
        }

        public async Task<bool> HasFlightsByUserAsync(string userId)
        {
            return await _context.Flights.AnyAsync(f => f.User.Id == userId);
        }
    }
}
