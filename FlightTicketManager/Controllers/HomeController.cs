using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FlightTicketManager.Models;
using FlightTicketManager.Data.Repositories;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace FlightTicketManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICityRepository _cityRepository;
        private readonly IFlightRepository _flightRepository;

        public HomeController(
            ILogger<HomeController> logger,
            ICityRepository cityRepository,
            IFlightRepository flightRepository)
        {
            _logger = logger;
            _cityRepository = cityRepository;
            _flightRepository = flightRepository;
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

        // GET: Flights
        public IActionResult AvailableFlights()
        {
            return View(_flightRepository.GetAvailableWithAircraftsAndCities());
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
