﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Data.Repositories;
using FlightTicketManager.Models;

namespace FlightTicketManager.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IUserHelper _userHelper;

        public ConverterHelper(IAircraftRepository aircraftRepository, IUserHelper userHelper)
        {
            _aircraftRepository = aircraftRepository;
            _userHelper = userHelper;
        }
        public Aircraft ToAircraft(AircraftViewModel model, string path, bool isNew)
        {
            return new Aircraft
            {
                Id = isNew ? 0 : model.Id,
                Description = model.Description,
                Airline = model.Airline,
                Capacity = model.Capacity,
                ImageUrl = path,
                IsActive = model.IsActive,
                Seats = model.Seats,
                User = model.User,
            };
        }

        public AircraftViewModel ToAircraftViewModel(Aircraft aircraft)
        {
            return new AircraftViewModel
            {
                Id = aircraft.Id,
                Description = aircraft.Description,
                Airline = aircraft.Airline,
                Capacity = aircraft.Capacity,
                ImageUrl = aircraft.ImageUrl,
                IsActive = aircraft.IsActive,
                Seats = aircraft.Seats,
                User = aircraft.User,
            };
        }

        public async Task<Flight> ToFlightAsync(FlightViewModel model, int aircraftId, User user)
        {
            var aircraft = await _aircraftRepository.GetByIdWithTrackingAsync(aircraftId);

            return new Flight
            {
                Id = model.Id,
                DepartureDateTime = model.DepartureDateTime,
                FlightDuration = model.FlightDuration,
                Origin = model.SelectedOrigin,
                Destination = model.SelectedDestination,
                Aircraft = aircraft,
                User = user,
                AvailableSeats = aircraft.Seats != null ? new List<string>(aircraft.Seats) : new List<string>()
            };
        }

        public async Task<FlightViewModel> ToFlightViewModel(Flight flight, int aircraftId, User flightUser)
        {
            var aircraft = await _aircraftRepository.GetByIdWithTrackingAsync(aircraftId);
            var user = await _userHelper.GetUserByEmailAsync(flightUser.UserName);

            return new FlightViewModel
            {
                Id = flight.Id,
                DepartureDateTime = flight.DepartureDateTime,
                FlightDuration = flight.FlightDuration,
                SelectedOrigin = flight.Origin,
                SelectedDestination = flight.Destination,
                SelectedAircraft = aircraft.Id,
                User = user,
                AvailableSeats = flight.AvailableSeats != null ? flight.AvailableSeats : new List<string>()
            };
        }
    }
}
