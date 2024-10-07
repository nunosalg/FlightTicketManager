using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlightTicketManager.Models;
using FlightTicketManager.Data.Repositories;
using FlightTicketManager.Helpers;
using FlightTicketManager.Services;
using Microsoft.AspNetCore.Authorization;

namespace FlightTicketManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICityRepository _cityRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketHistoryRepository _ticketHistoryRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IHistoryService _historyService;

        public HomeController(
            ILogger<HomeController> logger,
            ICityRepository cityRepository,
            IFlightRepository flightRepository,
            ITicketRepository ticketRepository,
            ITicketHistoryRepository ticketHistoryRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            IHistoryService historyService)
        {
            _logger = logger;
            _cityRepository = cityRepository;
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
            _ticketHistoryRepository = ticketHistoryRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _historyService = historyService;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            var model = new FlightSearchViewModel
            {
                Cities = _cityRepository.GetAll().Select(city => new SelectListItem
                {
                    Value = city.Id.ToString(),
                    Text = city.Name
                }).ToList()
            };
            return View(model);
        }

        // POST: Home/SearchFlights
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchFlights(FlightSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var flights = await _flightRepository.GetFlightsByCriteriaAsync(
                    model.SelectedOrigin,
                    model.SelectedDestination,
                    model.DepartureDateTime
                );

                model.Cities = _cityRepository.GetAll().Select(city => new SelectListItem
                {
                    Value = city.Id.ToString(),
                    Text = city.Name
                }).ToList();

                model.FlightsResults = flights;

                return View("Index", model);
            }

            return View("Index");
        }

        // GET: Home/Flights
        public IActionResult AvailableFlights()
        {
            return View(_flightRepository.GetAvailableWithAircraftsAndCities());
        }

        // GET: Home/BuyTicket
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> BuyTicket(int? id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = Url.Action("BuyTicket", "Home", new { id }) });
            }

            if (id == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var flight = await _flightRepository.GetByIdWithUsersAircraftsAndCitiesAsync(id.Value);
            if (flight == null)
            {
                return new NotFoundViewResult("FlightNotFound");
            }

            var model = new BuyTicketViewModel
            {
                FlightId = flight.Id,
                Flight = flight,
                AvailableSeats = flight.AvailableSeats, 
                Buyer = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name),
            };
            model.AvailableSeats.Sort();
            model.SetTicketPrice();

            return View(model);
        
        }

        // POST: Home/BuyTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyTicket(BuyTicketViewModel model)
        {
            // Remove ModelState errors for complex properties
            ModelState.Remove("Flight.User");
            ModelState.Remove("Flight.Aircraft");
            ModelState.Remove("Flight.Origin.CountryCode");
            ModelState.Remove("Flight.Destination.CountryCode");

            if (ModelState.IsValid)
            {
                var flight = await _flightRepository.GetByIdWithUsersAircraftsAndCitiesAsync(model.FlightId);
                if (flight == null)
                {
                    return new NotFoundViewResult("FlightNotFound");
                }

                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (await _ticketRepository.PassengerAlreadyHasTicketInFlight(flight.Id, model.PassengerId))
                {
                    model.Flight = flight;
                    model.PassengerName = model.PassengerName;
                    model.PassengerId = model.PassengerId;
                    model.PassengerBirthDate = model.PassengerBirthDate;
                    model.AvailableSeats = flight.AvailableSeats;

                    ModelState.AddModelError(string.Empty, $"The passenger with ID {model.PassengerId} already has a ticket for this flight.");

                    return View(model);
                }

                var ticket = await _converterHelper.ToTicketAsync(model, user, flight.Id);

                flight.AvailableSeats.Remove(model.Seat);
                flight.TicketsList.Add(ticket);

                await _ticketRepository.CreateAsync(ticket);
                await _flightRepository.UpdateAsync(flight);

                return RedirectToAction("TicketConfirmation", new { id = ticket.Id });
            }
            
            model.AvailableSeats = (await _flightRepository.GetByIdAsync(model.FlightId)).AvailableSeats;

            return View(model);
        }

        // GET: Home/TicketConfirmation
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> TicketConfirmation(int id)
        {
            var ticket = await _ticketRepository.GetByIdWithFlightDetailsAsync(id);
            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return View(ticket);
        }

        // GET: Home/MyFlights
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyFlights()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var tickets = _ticketRepository.GetTicketsByUserEmail(user.Email).ToList();

            return View(tickets);
        }

        // GET: Home/MyFlightsHistory
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyFlightsHistory()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var tickets = _ticketHistoryRepository.GetByUserId(user.Email).ToList();

            return View(tickets);
        }

        // GET: Home/ChangeSeat
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ChangeSeat(int id)
        {
            var ticket = await _ticketRepository.GetByIdWithFlightDetailsAsync(id);
            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var model = new ChangeSeatViewModel
            {
                TicketId = ticket.Id,
                FlightId = ticket.Flight.Id,
                AvailableSeats = ticket.Flight.AvailableSeats,
                CurrentSeat = ticket.Seat
            };
            model.AvailableSeats.Sort();

            return View(model);
        }

        // POST: Home/ChangeSeat
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSeat(ChangeSeatViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ticket = await _ticketRepository.GetByIdWithFlightDetailsAsync(model.TicketId);
                if (ticket == null)
                {
                    return new NotFoundViewResult("TicketNotFound");
                }

                // Update seat logic
                var flight = ticket.Flight;
                flight.AvailableSeats.Remove(model.NewSeat);
                flight.AvailableSeats.Add(ticket.Seat); // Re-add the old seat

                ticket.Seat = model.NewSeat;
                await _ticketRepository.UpdateAsync(ticket);
                await _flightRepository.UpdateAsync(flight);

                return RedirectToAction(nameof(MyFlights)); 
            }

            return View(model);
        }

        // GET: Home/CancelTicket
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelTicket(int? id)
        {
            var ticket = await _ticketRepository.GetByIdWithFlightDetailsAsync(id.Value);
            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            return View(ticket);
        }

        // POST: Home/CancelTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelTicket(int id)
        {
            var ticket = await _ticketRepository.GetByIdWithFlightDetailsAsync(id);
            if (ticket == null)
            {
                return new NotFoundViewResult("TicketNotFound");
            }

            var flight = ticket.Flight;
            flight.AvailableSeats.Add(ticket.Seat);

            // Save the ticket history before updating the flight
            await _historyService.SaveTicketHistoryAsync(ticket, "Cancelled");

            // Update the flight and delete the ticket
            await _flightRepository.UpdateAsync(flight);
            await _ticketRepository.DeleteAsync(ticket);

            return RedirectToAction(nameof(MyFlights)); 
        }

        public IActionResult TicketNotFound()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }
    }
}
