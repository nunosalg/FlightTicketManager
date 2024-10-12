using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using FlightTicketManager.Data.Repositories;
using FlightTicketManager.Helpers;
using FlightTicketManager.Models;
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
        private readonly IFlightHelper _flightHelper;
        private readonly IHistoryService _historyService;
        private readonly AirportsApiService _airportsApiService;

        public FlightsController(
            IFlightRepository flightRepository,
            ICityRepository cityRepository,
            IAircraftRepository aircraftRepository,
            ITicketRepository ticketRepository,
            IFlightHistoryRepository flightHistoryRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            IMailHelper mailHelper,
            IFlightHelper flightHelper,
            IHistoryService historyService,
            AirportsApiService airportsApiService
            )
        {
            _flightRepository = flightRepository;
            _userHelper = userHelper;
            _cityRepository = cityRepository;
            _aircraftRepository = aircraftRepository;
            _ticketRepository = ticketRepository;
            _flightHistoryRepository = flightHistoryRepository;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _flightHelper = flightHelper;
            _historyService = historyService;
            _airportsApiService = airportsApiService;
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
                    Text = aircraft.ModelId,
                }),

                Airports = new List<SelectListItem>()
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

                    var validationResult = _flightHelper.AircraftHasOverlappingFlights(
                        model.DepartureDateTime,
                        selectedOrigin.Name,
                        selectedDestination.Name,
                        model.FlightDuration,
                        selectedAircraft
                    );

                    if (validationResult.IsValid)
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
                    else
                    {
                        ModelState.AddModelError("", validationResult.Error);
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model.Cities = _cityRepository.GetAll().Select(city => new SelectListItem
            {
                Value = city.Id.ToString(),
                Text = city.Name,
            }).ToList();

            model.Aircrafts = _aircraftRepository.GetAllActive().Select(aircraft => new SelectListItem
            {
                Value = aircraft.Id.ToString(),
                Text = aircraft.ModelId,
            }).ToList();

            model.Airports = new List<SelectListItem>();

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

            // Pre-set the selected airports in the model
            model.SelectedOriginAirport = flight.OriginAirport;  
            model.SelectedDestinationAirport = flight.DestinationAirport;

            _flightHelper.LoadCitiesAndAircrafts(flight, model);

            model.Airports = new List<SelectListItem>();

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

                        _flightHelper.LoadCitiesAndAircrafts(currentFlight, model);

                        return View(model);
                    }

                    model.TicketsList = currentFlight.TicketsList;

                    var validationResult = _flightHelper.AircraftHasOverlappingFlights(
                        model.DepartureDateTime,
                        selectedOrigin.Name,
                        selectedDestination.Name,
                        model.FlightDuration,
                        selectedAircraft,
                        currentFlight
                    );

                    if (validationResult.IsValid)
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
                        foreach (var ticket in flight.TicketsList)
                        {
                            flight.AvailableSeats.Remove(ticket.Seat);
                        }

                        await _flightRepository.UpdateAsync(flight);
                    }
                    else
                    {
                        ModelState.AddModelError("", validationResult.Error);
                        return View(model);
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

        [HttpGet]
        public async Task<JsonResult> GetOriginAirports(int cityId)
        {
            var city = await _cityRepository.GetByIdAsync(cityId);
            if (city == null)
            {
                return Json(new { success = false, message = "City not found" });
            }

            var response = await _airportsApiService.GetAirportsAsync(city.Name);
            if (response.IsSuccess)
            {
                var airports = ((List<AirportApi>)response.Results).Select(a => new SelectListItem
                {
                    Value = a.Name,
                    Text = $"{a.Name} - {a.City}, {a.Country}"
                }).ToList();

                return Json(new { success = true, airports });
            }

            return Json(new { success = false, message = response.Message });
        }

        [HttpGet]
        public async Task<JsonResult> GetDestinationAirports(int cityId)
        {
            var city = await _cityRepository.GetByIdAsync(cityId);
            if (city == null)
            {
                return Json(new { success = false, message = "City not found" });
            }

            var response = await _airportsApiService.GetAirportsAsync(city.Name);
            if (response.IsSuccess)
            {
                var airports = ((List<AirportApi>)response.Results).Select(a => new SelectListItem
                {
                    Value = a.Name,
                    Text = $"{a.Name} - {a.City}, {a.Country}"
                }).ToList();

                return Json(new { success = true, airports });
            }

            return Json(new { success = false, message = response.Message });
        }

        public IActionResult FlightsHistory()
        {
            return View(_flightHistoryRepository.GetAll());
        }

        public IActionResult FlightNotFound()
        {
            return View();
        }
    }
}
