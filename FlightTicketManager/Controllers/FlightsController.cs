using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FlightTicketManager.Data.Repositories;
using FlightTicketManager.Helpers;
using FlightTicketManager.Models;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Services;

namespace FlightTicketManager.Controllers
{
    [Authorize(Roles = "Employee")]
    public class FlightsController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IUserHelper _userHelper;
        private readonly ICityRepository _cityRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IFlightHistoryRepository _flightHistoryRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IHistoryService _historyService;

        public FlightsController(
            IFlightRepository flightRepository,
            ICityRepository cityRepository,
            IAircraftRepository aircraftRepository,
            ITicketRepository ticketRepository,
            IFlightHistoryRepository flightHistoryRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            IMailHelper mailHelper,
            IHistoryService historyService)
        {
            _flightRepository = flightRepository;
            _userHelper = userHelper;
            _cityRepository = cityRepository;
            _aircraftRepository = aircraftRepository;
            _ticketRepository = ticketRepository;
            _flightHistoryRepository = flightHistoryRepository;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _historyService = historyService;
        }

        // GET: Flights
        public IActionResult Index()
        {
            return View(_flightRepository.GetAvailableWithUsersAircraftsAndCities());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetByIdWithUsersAircraftsAndCitiesAsync(id.Value);
            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            return View(flight);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            var model = new FlightViewModel
            {
                Cities = _cityRepository.GetAll().Select(city => new SelectListItem
                {
                    Value = city.Id.ToString(),
                    Text = city.Name,
                }).ToList(),

                Aircrafts = _aircraftRepository.GetAllActive().Select(aircraft => new SelectListItem
                {
                    Value = aircraft.Id.ToString(),
                    Text = aircraft.Data, 
                })
            };
            return View(model);
        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlightViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    var selectedAircraft = await _aircraftRepository.GetByIdAsync(model.SelectedAircraft);
                    var selectedOrigin = await _cityRepository.GetByIdAsync(model.SelectedOrigin);
                    var selectedDestination = await _cityRepository.GetByIdAsync(model.SelectedDestination);

                    if (!AircraftHasOverlappingFlights(
                        model.DepartureDateTime, 
                        selectedOrigin.Name, 
                        selectedDestination.Name, 
                        model.FlightDuration,
                        selectedAircraft))
                    {
                        var flight = await _converterHelper.ToFlightAsync(
                            model, 
                            model.SelectedOrigin, 
                            model.SelectedDestination, 
                            model.SelectedAircraft, 
                            model.User, 
                            model.TicketsList
                        );

                        await _flightRepository.CreateAsync(flight);

                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateException)
                {
                    
                    ModelState.AddModelError("", "Flight duration can't be longer than 24 hours");
                }
            }

            model.Cities = _cityRepository.GetAll().Select(city => new SelectListItem
            {
                Value = city.Id.ToString(),
                Text = city.Name,
            }).ToList();

            model.Aircrafts = _aircraftRepository.GetAll().Select(aircraft => new SelectListItem
            {
                Value = aircraft.Id.ToString(),
                Text = aircraft.Description,
            }).ToList();

            return View(model);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetByIdWithUsersAircraftsAndCitiesAsync(id.Value);
            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var model = await _converterHelper.ToFlightViewModelAsync(flight, flight.Aircraft.Id, flight.User, flight.TicketsList);

            LoadCitiesAndAircrafts(flight, model);

            return View(model);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlightViewModel model)
        {
            if (id != model.Id)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    var selectedAircraft = await _aircraftRepository.GetByIdAsync(model.SelectedAircraft);
                    var selectedOrigin = await _cityRepository.GetByIdAsync(model.SelectedOrigin);
                    var selectedDestination = await _cityRepository.GetByIdAsync(model.SelectedDestination);
                    var currentFlight = await _flightRepository.GetByIdWithUsersAircraftsAndCitiesAsync(id);

                    if (currentFlight.TicketsList.Count > 0 && selectedAircraft.Capacity < currentFlight.Aircraft.Capacity)
                    {
                        ModelState.AddModelError(string.Empty, "The selected aircraft has a smaller capacity than the actual aircraft " +
                            "and this flight already has tickets sold");

                        LoadCitiesAndAircrafts(currentFlight, model);
                        
                        return View(model);
                    }

                    model.TicketsList = currentFlight.TicketsList;

                    if (!AircraftHasOverlappingFlights(
                        model.DepartureDateTime,
                        selectedOrigin.Name,
                        selectedDestination.Name,
                        model.FlightDuration,
                        selectedAircraft,
                        currentFlight))
                    {
                        var flight = await _converterHelper.ToFlightAsync(
                            model, 
                            model.SelectedOrigin,
                            model.SelectedDestination,
                            model.SelectedAircraft,
                            model.User,
                            model.TicketsList,
                            currentFlight
                        );

                        flight.InitializeAvailableSeats();
                        foreach(var ticket in flight.TicketsList)
                        {
                            flight.AvailableSeats.Remove(ticket.Seat);
                        }
                        
                        await _flightRepository.UpdateAsync(flight);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _flightRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("FlightNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetByIdWithUsersAircraftsAndCitiesAsync(id.Value);
            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _flightRepository.GetByIdWithTrackingAsync(id);
            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var tickets = await _ticketRepository.GetTicketsByFlightIdAsync(id);

            if (tickets != null && tickets.Any())
            {
                // Notify users about ticket cancellation and refund
                foreach (var ticket in tickets)
                {
                    await _mailHelper.SendCancellationEmailAsync(ticket.TicketBuyer.Email, flight.FlightNumber, ticket.Price);
                }

                // Only save flight history if the flight has any tickets sold (also saves tickets from the flight list)
                await _historyService.SaveFlightHistoryAsync(flight, "Cancelled", "Refunded");
            }

            await _flightRepository.DeleteAsync(flight);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult FlightsHistory()
        {
            return View(_flightHistoryRepository.GetAll());
        }

        public IActionResult FlightNotFound()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedDate"></param>
        /// <param name="selectedOrigin"></param>
        /// <param name="selectedDestination"></param>
        /// <param name="flightDuration"></param>
        /// <param name="selectedAircraft"></param>
        /// <param name="currentFlight"></param>
        /// <returns></returns>
        private bool AircraftHasOverlappingFlights(
            DateTime selectedDate, 
            string selectedOrigin, 
            string selectedDestination, 
            TimeSpan flightDuration, 
            Aircraft selectedAircraft, 
            Flight currentFlight = null)
        {
            int preparationTime = 60; // Preparation time for aircraft in minutes
            int totalFlightMinutes = (int)flightDuration.TotalMinutes;

            var conflictingFlights = _flightRepository.GetConflictingFlights(
                selectedAircraft,
                selectedDate,
                selectedOrigin,
                selectedDestination,
                currentFlight);

            // If any flight exists with a different route on the same day
            if (conflictingFlights.Count > 0)
            {
                ModelState.AddModelError(string.Empty, "The aircraft has a different route scheduled on this day.");
                return true;
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
                    ModelState.AddModelError(string.Empty, "The aircraft is already scheduled for a flight on this route at the same time.");
                    return true;
                }

                // If the flight has the same origin and destination
                if (selectedOrigin == flight.Origin.Name && selectedDestination == flight.Destination.Name)
                {
                    int preparationAndFlightTime = (totalFlightMinutes + preparationTime) * 2;

                    // Check for overlapping flights
                    if (newDeparture < existingDeparture && newDeparture > existingDeparture.AddMinutes(-preparationAndFlightTime))
                    {
                        ModelState.AddModelError(string.Empty, "The aircraft is scheduled for a flight on this route around the same time.");
                        return true;
                    }
                    if (newDeparture > existingDeparture && newDeparture < existingDeparture.AddMinutes(preparationAndFlightTime))
                    {
                        ModelState.AddModelError(string.Empty, "The aircraft is scheduled for a flight on this route around the same time.");
                        return true;
                    }
                }
                // If the flight has swapped origin and destination
                else if (selectedOrigin == flight.Destination.Name && selectedDestination == flight.Origin.Name)
                {
                    // Check for overlapping flights
                    if (newDeparture < existingDeparture && newDeparture > existingDeparture.AddMinutes(-totalFlightMinutes - preparationTime))
                    {
                        ModelState.AddModelError(string.Empty, "The aircraft is scheduled for a flight with a different route around the same time.");
                        return true;
                    }
                    if (newDeparture > existingDeparture && newDeparture < existingArrival.AddMinutes(preparationTime))
                    {
                        ModelState.AddModelError(string.Empty, "The aircraft is scheduled for a flight with a different route around the same time.");
                        return true;
                    }
                }
            }

            return false; // No conflicts, flight can be created
        }

        /// <summary>
        /// 
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
    }
}
