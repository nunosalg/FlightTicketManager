using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Data.Repositories;
using FlightTicketManager.Models;
using static Google.Apis.Requests.BatchRequest;
using System.Collections.Generic;
using FlightTicketManager.Services;
using System.Threading.Tasks;

namespace FlightTicketManager.Helpers
{
    public class FlightHelper : IFlightHelper
    {
        private readonly ICityRepository _cityRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly AirportsApiService _airportsApiService;

        public FlightHelper(
            ICityRepository cityRepository, 
            IAircraftRepository aircraftRepository, 
            IFlightRepository flightRepository,
            AirportsApiService airportsApiService)
        {
            _cityRepository = cityRepository;
            _aircraftRepository = aircraftRepository;
            _flightRepository = flightRepository;
            _airportsApiService = airportsApiService;
        }

        /// <summary>
        /// Loads cities to a Select List Item and Aircrafts to another Select List Item
        /// </summary>
        /// <param name="flight"></param>
        /// <param name="model"></param>
        public void LoadCitiesAndAircrafts(Flight currentFlight, FlightViewModel model)
        {
            model.Cities = _cityRepository.GetAll().Select(city => new SelectListItem
            {
                Value = city.Id.ToString(),
                Text = city.Name,
                Selected = city.Id == currentFlight.Origin.Id || city.Id == currentFlight.Destination.Id
            }).ToList();

            model.Aircrafts = _aircraftRepository.GetAllActive().Select(aircraft => new SelectListItem
            {
                Value = aircraft.Id.ToString(),
                Text = aircraft.Data,
                Selected = aircraft.Id == currentFlight.Aircraft.Id
            }).ToList();

            model.SelectedOrigin = currentFlight.Origin.Id;
            model.SelectedDestination = currentFlight.Destination.Id;
            model.SelectedAircraft = currentFlight.Aircraft.Id;
        }

        //public async Task<JsonResult> GetAirportsAsync()
        //{

        //}

        /// <summary>
        /// Checks if an aircraft has overlaping flights
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <param name="selectedOrigin"></param>
        /// <param name="selectedDestination"></param>
        /// <param name="flightDuration"></param>
        /// <param name="selectedAircraft"></param>
        /// <param name="currentFlight"></param>
        /// <returns></returns>
        public ValidationResult AircraftHasOverlappingFlights(
            DateTime selectedDate,
            string selectedOrigin,
            string selectedDestination,
            TimeSpan flightDuration,
            Aircraft selectedAircraft,
            Flight currentFlight = null)
        {
            int preparationTime = 60; // Preparation time for aircraft in minutes
            int totalFlightMinutes = (int)flightDuration.TotalMinutes;
            var result = new ValidationResult();

            var conflictingFlights = _flightRepository.GetConflictingFlights(
                selectedAircraft,
                selectedDate,
                selectedOrigin,
                selectedDestination,
                currentFlight
            );

            // If any flight exists with a different route on the same day
            if (conflictingFlights.Count > 0)
            {
                result.AddError("The aircraft has a different route scheduled on this day.");
                return result;
            }

            // Get flights on the same date with the same aircraft
            var sameDayFlights = _flightRepository.GetSameDayFlights(selectedAircraft, selectedDate, currentFlight);

            foreach (var flight in sameDayFlights)
            {
                DateTime existingDeparture = flight.DepartureDateTime;
                DateTime existingArrival = flight.DepartureDateTime + flight.FlightDuration;
                DateTime newDeparture = selectedDate;

                // If the new flight starts at the same time as an existing one
                if (newDeparture == existingDeparture)
                {
                    result.AddError("The aircraft is already scheduled for a flight on this route at the same time.");
                    return result;
                }

                // If the flight has the same origin and destination
                if (selectedOrigin == flight.Origin.Name && selectedDestination == flight.Destination.Name)
                {
                    int preparationAndFlightTime = (totalFlightMinutes + preparationTime) * 2;

                    // Check for overlapping flights
                    if (newDeparture < existingDeparture && newDeparture > existingDeparture.AddMinutes(-preparationAndFlightTime))
                    {
                        result.AddError("The aircraft is scheduled for a flight on this route around the same time.");
                        return result;
                    }
                    if (newDeparture > existingDeparture && newDeparture < existingDeparture.AddMinutes(preparationAndFlightTime))
                    {
                        result.AddError("The aircraft is scheduled for a flight on this route around the same time.");
                        return result;
                    }
                }
                // If the flight has swapped origin and destination
                else if (selectedOrigin == flight.Destination.Name && selectedDestination == flight.Origin.Name)
                {
                    // Check for overlapping flights
                    if (newDeparture < existingDeparture && newDeparture > existingDeparture.AddMinutes(-totalFlightMinutes - preparationTime))
                    {
                        result.AddError("The aircraft is scheduled for a flight with a different route around the same time.");
                        return result;
                    }
                    if (newDeparture > existingDeparture && newDeparture < existingArrival.AddMinutes(preparationTime))
                    {
                        result.AddError("The aircraft is scheduled for a flight with a different route around the same time.");
                        return result;
                    }
                }
            }

            return result; // No conflicts, flight can be created
        }
    }
}
