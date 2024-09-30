using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Helpers;
using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IFlightRepository _flightRepository;
        private Random _random;

        public SeedDb(
            DataContext context, 
            IUserHelper userHelper,
            IAircraftRepository aircraftRepository,
            IFlightRepository flightRepository)
        {
            _context = context;
            _userHelper = userHelper;
            _aircraftRepository = aircraftRepository;
            _flightRepository = flightRepository;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Employee");
            await _userHelper.CheckRoleAsync("Customer");

            var user = await _userHelper.GetUserByEmailAsync("nunotestescet87@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Nuno",
                    LastName = "Salgueiro",
                    Email = "nunotestescet87@gmail.com",
                    UserName = "nunotestescet87@gmail.com",
                    BirthDate = new DateTime(1990, 10, 24),
                    IdNumber = "12345678"
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_context.Aircrafts.Any())
            {
                AddAircraft("Airbus319", "TAP", user);
                AddAircraft("Airbus329", "Ryanair", user);
                AddAircraft("Airbus339", "EasyJet", user);
                AddAircraft("Airbus339", "TAP", user);

                await _context.SaveChangesAsync();
            }

            if (!_context.Cities.Any())
            {
                AddCity("Lisbon", "PT");
                AddCity("Madrid", "ES");
                AddCity("Rome", "IT");
                AddCity("Paris", "FR");
                AddCity("Oslo", "NO");

                await _context.SaveChangesAsync();
            }

            if (!_context.Flights.Any())
            {
                var departureDateTime = new DateTime(2024, 9, 12, 14, 30, 0); 
                var flightDuration = new TimeSpan(2, 30, 0);
                await AddFlight(departureDateTime, flightDuration, 1, 3, 3, user);
                
                departureDateTime = new DateTime(2024, 10, 11, 8, 30, 0);
                flightDuration = new TimeSpan(1, 30, 0);
                await AddFlight(departureDateTime, flightDuration, 1, 4, 2, user);

                departureDateTime = new DateTime(2024, 12, 11, 7, 30, 0);
                flightDuration = new TimeSpan(4, 30, 0);
                await AddFlight(departureDateTime, flightDuration, 1, 5, 1, user);

                await _context.SaveChangesAsync();
            }

            if(!_context.Tickets.Any())
            {
                var passengerBirthdate = new DateTime(2005, 9, 12);
                await AddTicket(1, "01A", user, "14151617", "Jose Maria", passengerBirthdate);
                await AddTicket(2, "01A", user, "14151617", "Jose Maria", passengerBirthdate);
                await AddTicket(3, "01A", user, "14151617", "Jose Maria", passengerBirthdate);
            }
        }

        private async Task AddTicket(int flightId, string seat, User user, string passengerId, string passengerName, DateTime passengerBirthdate)
        {
            var flight = _context.Flights.Local.FirstOrDefault(f => f.Id == flightId);

            if (flight == null)
            {
                flight = await _flightRepository.GetByIdAsync(flightId);
                _context.Flights.Attach(flight);
            }

            var ticket = new Ticket
            {
                Flight = flight,
                Seat = seat,
                TicketBuyer = user,
                PassengerId = passengerId,
                PassengerName = passengerName,
                PassengerBirthDate = passengerBirthdate,
            };

            flight.AvailableSeats.Remove(seat);
            flight.TicketsList.Add(ticket);

            _context.Tickets.Add(ticket);

            await _flightRepository.UpdateAsync(flight);
        }

        private void AddCity(string name, string countryCode)
        {
            _context.Cities.Add(new City
            {
                Name = name,
                CountryCode = countryCode
            });
        }

        private void AddAircraft(string description, string airline, User user)
        {
            var aircraft = new Aircraft
            {
                Description = description,
                Airline = airline,
                Capacity = _random.Next(150, 300),
                IsActive = true,
                User = user
            };

            aircraft.GenerateSeats();
            _context.Aircrafts.Add(aircraft);
        }

        private async Task AddFlight(DateTime departureDateTime, TimeSpan duration, int originId, int destinationId, int aircraftId, User user)
        {
            var aircraft = _context.Aircrafts.Local.FirstOrDefault(a => a.Id == aircraftId);
            var origin = _context.Cities.Local.FirstOrDefault(o => o.Id == originId);
            var destination = _context.Cities.Local.FirstOrDefault(d => d.Id == destinationId);

            if (aircraft == null)
            {
                // Get the aircraft in the database and attach it to the context 
                aircraft = await _aircraftRepository.GetByIdAsync(aircraftId);
                _context.Aircrafts.Attach(aircraft);
            }

            var flight = new Flight
            {
                DepartureDateTime = departureDateTime,
                FlightDuration = duration,
                Origin = origin,
                Destination = destination,
                Aircraft = aircraft,
                User = user,
                TicketsList = new List<Ticket>()
            };

            flight.InitializeAvailableSeats();
            _context.Flights.Add(flight);
        }
    }
}
