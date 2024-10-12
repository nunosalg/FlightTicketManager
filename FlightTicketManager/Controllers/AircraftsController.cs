using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Helpers;
using FlightTicketManager.Models;
using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AircraftsController : Controller
    {
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public AircraftsController(
            IAircraftRepository aircraftRepository,
            IFlightRepository flightRepository,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _aircraftRepository = aircraftRepository;
            _flightRepository = flightRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Aircrafts
        public IActionResult Index()
        {
            return View(_aircraftRepository.GetAll().OrderBy(a => a.Model));
        }

        // GET: Aircrafts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AircraftNotFound");
            }

            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return new NotFoundViewResult("AircraftNotFound");
            }

            return View(aircraft);
        }

        // GET: Aircrafts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aircrafts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AircraftViewModel aircraftModel)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (aircraftModel.ImageFile != null && aircraftModel.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(aircraftModel.ImageFile, "aircrafts");
                }

                var aircraft = _converterHelper.ToAircraft(aircraftModel, path, true);
                aircraft.GenerateSeats();

                aircraft.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                await _aircraftRepository.CreateAsync(aircraft);
                return RedirectToAction(nameof(Index));
            }
            return View(aircraftModel);
        }

        // GET: Aircrafts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AircraftNotFound");
            }

            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return new NotFoundViewResult("AircraftNotFound");
            }

            var model = _converterHelper.ToAircraftViewModelAsync(aircraft);
            return View(model);
        }

        // POST: Aircrafts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AircraftViewModel aircraftModel)
        {
            if (ModelState.IsValid)
            {
                var hasFlights = await _flightRepository.HasFlightsWithAircraftAsync(aircraftModel.Id);

                if (hasFlights)
                {
                    ModelState.AddModelError(string.Empty, "This aircraft cannot be edited because it is associated with one or more flights.");
                    return View(aircraftModel);
                }

                try
                {
                    var path = aircraftModel.ImageUrl;

                    if (aircraftModel.ImageFile != null && aircraftModel.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(aircraftModel.ImageFile, "aircrafts");
                    }

                    var aircraft = _converterHelper.ToAircraft(aircraftModel, path, false);
                    aircraft.UpdateCapacity(aircraftModel.Capacity);

                    aircraft.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _aircraftRepository.UpdateAsync(aircraft);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aircraftRepository.ExistAsync(aircraftModel.Id))
                    {
                        return new NotFoundViewResult("AircraftNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aircraftModel);
        }

        // GET: Aircrafts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AircraftNotFound");
            }

            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return new NotFoundViewResult("AircraftNotFound");
            }

            return View(aircraft);
        }

        // POST: Aircrafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aircraft = await _aircraftRepository.GetByIdAsync(id);

            var hasFlights = await _flightRepository.HasFlightsWithAircraftAsync(id); 

            if (hasFlights)
            {
                ModelState.AddModelError(string.Empty, "This aircraft cannot be deleted because it is associated with one or more flights.");
                return View(aircraft); 
            }

            await _aircraftRepository.DeleteAsync(aircraft);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AircraftNotFound()
        {
            return View();
        }
    }
}
