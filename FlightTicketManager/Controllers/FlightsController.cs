using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FlightTicketManager.Data.Repositories;
using FlightTicketManager.Helpers;
using FlightTicketManager.Models;

namespace FlightTicketManager.Controllers
{
    [Authorize(Roles = "Employee")]
    public class FlightsController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IUserHelper _userHelper;
        private readonly ICityRepository _cityRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IConverterHelper _converterHelper;

        public FlightsController(
            IFlightRepository flightRepository,
            IUserHelper userHelper,
            ICityRepository cityRepository,
            IAircraftRepository aircraftRepository,
            IConverterHelper converterHelper)
        {
            _flightRepository = flightRepository;
            _userHelper = userHelper;
            _cityRepository = cityRepository;
            _aircraftRepository = aircraftRepository;
            _converterHelper = converterHelper;
        }

        // GET: Flights
        public IActionResult Index()
        {
            return View(_flightRepository.GetAllWithUsersAircraftsAndCities());
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
                    Text = aircraft.AircraftData, 
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

            model.Cities = _cityRepository.GetAll().Select(city => new SelectListItem
            {
                Value = city.Id.ToString(),
                Text = city.Name,
                Selected = city.Id == flight.Origin.Id || city.Id == flight.Destination.Id
            }).ToList();

            model.Aircrafts = _aircraftRepository.GetAllActive().Select(aircraft => new SelectListItem
            {
                Value = aircraft.Id.ToString(),
                Text = aircraft.AircraftData,
                Selected = aircraft.Id == flight.Aircraft.Id
            }).ToList();

            model.SelectedOrigin = flight.Origin.Id;
            model.SelectedDestination = flight.Destination.Id;
            model.SelectedAircraft = flight.Aircraft.Id;

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

                    var flight = await _converterHelper.ToFlightAsync(
                        model, model.SelectedOrigin, 
                        model.SelectedDestination, 
                        model.SelectedAircraft, 
                        model.User,
                        model.TicketsList
                    );

                    flight.InitializeAvailableSeats();

                    await _flightRepository.UpdateAsync(flight);
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
            var flight = await _flightRepository.GetByIdAsync(id);
            await _flightRepository.DeleteAsync(flight);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult FlightNotFound()
        {
            return View();
        }
    }
}
