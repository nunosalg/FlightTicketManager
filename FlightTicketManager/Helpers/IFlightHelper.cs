using System;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Models;

namespace FlightTicketManager.Helpers
{
    public interface IFlightHelper
    {
        void LoadCitiesAndAircrafts(Flight currentFlight, FlightViewModel model);

        ValidationResult AircraftHasOverlappingFlights(
            DateTime selectedDate,
            string selectedOrigin,
            string selectedDestination,
            TimeSpan flightDuration,
            Aircraft selectedAircraft,
            Flight currentFlight = null);
    }
}
