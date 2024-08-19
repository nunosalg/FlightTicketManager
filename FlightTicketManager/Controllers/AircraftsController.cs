using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Helpers;
using FlightTicketManager.Models;
using System;

namespace FlightTicketManager.Controllers
{
    public class AircraftsController : Controller
    {
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IUserHelper _userHelper;

        public AircraftsController(IAircraftRepository aircraftRepository, IUserHelper userHelper)
        {
            _aircraftRepository = aircraftRepository;
            _userHelper = userHelper;
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
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\aircrafts",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/aircrafts/{file}";
                }

                var aircraft = this.ToAircraft(model, path);

                //TODO: Modificar para o user que estiver logado
                aircraft.User = await _userHelper.GetUserByEmailAsync("nunosalgueiro23@gmail.com");

                await _aircraftRepository.CreateAsync(aircraft);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private Aircraft ToAircraft(AircraftViewModel model, string path)
        {
            return new Aircraft
            {
                Id = model.Id,
                Description = model.Description,
                Airline = model.Airline,
                Capacity = model.Capacity,
                ImageUrl = path,
                IsActive = model.IsActive,
                User = model.User,
            };
        }

        // GET: Aircrafts/Edit/5
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

            var model = this.ToAircraftViewModel(aircraft);
            return View(model);
        }

        private AircraftViewModel ToAircraftViewModel(Aircraft aircraft)
        {
            return new AircraftViewModel
            {
                Id = aircraft.Id,
                Description = aircraft.Description,
                Airline = aircraft.Airline,
                Capacity = aircraft.Capacity,
                ImageUrl = aircraft.ImageUrl,
                IsActive = aircraft.IsActive,
                User = aircraft.User,
            };
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
                        var guid = Guid.NewGuid().ToString();
                        var file = $"{guid}.jpg";

                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\aircrafts",
                            file);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/aircrafts/{file}";
                    }


                    var aircraft = this.ToAircraft(model, path);

                    //TODO: Modificar para o user que estiver logado
                    aircraft.User = await _userHelper.GetUserByEmailAsync("nunosalgueiro23@gmail.com");
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
