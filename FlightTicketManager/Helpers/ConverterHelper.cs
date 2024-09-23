using System.Collections.Generic;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Data.Repositories;
using FlightTicketManager.Models;

namespace FlightTicketManager.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly IAircraftRepository _aircraftRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IUserHelper _userHelper;

        public ConverterHelper(
            IAircraftRepository aircraftRepository, 
            ICityRepository cityRepository,
            IFlightRepository flightRepository,
            IUserHelper userHelper)
        {
            _aircraftRepository = aircraftRepository;
            _cityRepository = cityRepository;
            _flightRepository = flightRepository;
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

        public AircraftViewModel ToAircraftViewModelAsync(Aircraft aircraft)
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

        public async Task<Flight> ToFlightAsync(FlightViewModel model, int originId, int destinationId, int aircraftId, User user, List<Ticket> tickets)
        {
            var aircraft = await _aircraftRepository.GetByIdWithTrackingAsync(aircraftId);
            var origin = await _cityRepository.GetByIdWithTrackingAsync(originId);
            var destination = await _cityRepository.GetByIdWithTrackingAsync(destinationId);

            return new Flight
            {
                Id = model.Id,
                DepartureDateTime = model.DepartureDateTime,
                FlightDuration = model.FlightDuration,
                Origin = origin,
                Destination = destination,
                Aircraft = aircraft,
                User = user,
                AvailableSeats = aircraft.Seats != null ? new List<string>(aircraft.Seats) : new List<string>(),
                TicketsList = tickets,
            };
        }

        public async Task<FlightViewModel> ToFlightViewModelAsync(Flight flight, int aircraftId, User flightUser, List<Ticket> tickets)
        {
            var aircraft = await _aircraftRepository.GetByIdWithTrackingAsync(aircraftId);
            var user = await _userHelper.GetUserByEmailAsync(flightUser.UserName);

            return new FlightViewModel
            {
                Id = flight.Id,
                DepartureDateTime = flight.DepartureDateTime,
                FlightDuration = flight.FlightDuration,
                SelectedOrigin = flight.Origin.Id,
                SelectedDestination = flight.Destination.Id,
                SelectedAircraft = aircraft.Id,
                User = user,
                AvailableSeats = flight.AvailableSeats ?? new List<string>(),
                TicketsList = tickets
            };
        }

        public async Task<Ticket> ToTicketAsync(BuyTicketViewModel model, User clientUser, int flightId)
        {
            var user = await _userHelper.GetUserByEmailAsync(clientUser.UserName);
            var flight = await _flightRepository.GetByIdWithUsersAircraftsAndCitiesAsync(flightId);
            return new Ticket
            {
                Flight = flight,
                Seat = model.Seat,
                TicketBuyer = user,
                PassengerId = model.PassengerId,
                PassengerName = model.PassengerName,
                PassengerBirthDate = model.PassengerBirthDate,
            };
        }

        public TicketConfirmationViewModel ToTicketConfirmationViewModel(Ticket ticket)
        {
            return new TicketConfirmationViewModel
            {
                TicketId = ticket.Id,
                FlightNumber = ticket.Flight.FlightNumber,
                Origin = ticket.Flight.Origin.Name,
                Destination = ticket.Flight.Destination.Name,
                DepartureDateTime = ticket.Flight.DepartureDateTime,
                Seat = ticket.Seat,
                PassengerName = ticket.PassengerName,
                PassengerId = ticket.PassengerId,
                PassengerBirthDate = ticket.PassengerBirthDate,
            };
        }
    }
}
