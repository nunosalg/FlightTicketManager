using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data;
using FlightTicketManager.Helpers;
using FlightTicketManager.Models;

namespace FlightTicketManager.Controllers
{
    public class AircraftsController : Controller
    {
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public AircraftsController(
            IAircraftRepository aircraftRepository,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _aircraftRepository = aircraftRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        // GET: Aircrafts
        public IActionResult Index()
        {
            return View(_aircraftRepository.GetAll().OrderBy(a => a.Description));
        }

        // GET: Aircrafts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return NotFound();
            }

            return View(aircraft);
        }

        // GET: Aircrafts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aircrafts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AircraftViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "aircrafts");
                }

                var aircraft = _converterHelper.ToAircraft(model, path, true);

                aircraft.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                await _aircraftRepository.CreateAsync(aircraft);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Aircrafts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToAircraftViewModel(aircraft);
            return View(model);
        }

        // POST: Aircrafts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AircraftViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "aircrafts");
                    }

                    var aircraft = _converterHelper.ToAircraft(model, path, false);

                    aircraft.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _aircraftRepository.UpdateAsync(aircraft);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aircraftRepository.ExistAsync(model.Id))
                    {
                        return NotFound();
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

        // GET: Aircrafts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return NotFound();
            }

            return View(aircraft);
        }

        // POST: Aircrafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aircraft = await _aircraftRepository.GetByIdAsync(id);
            await _aircraftRepository.DeleteAsync(aircraft);

            return RedirectToAction(nameof(Index));
        }

    }
}
