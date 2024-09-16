using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Helpers;
using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CitiesController : Controller
    {
        private readonly ICityRepository _cityRepository;

        public CitiesController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        // GET: Cities
        public IActionResult Index()
        {
            return View(_cityRepository.GetAll().OrderBy(c => c.Name));
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            return View(city);
        }

        // GET: Cities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(City city)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    city.CountryCode = city.CountryCode.ToUpper();

                    await _cityRepository.CreateAsync(city);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    return new NotFoundViewResult("CityAlreadyExists");
                }
            }
            return View(city);
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }
            return View(city);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(City city)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    city.CountryCode = city.CountryCode.ToUpper();

                    await _cityRepository.UpdateAsync(city);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _cityRepository.ExistAsync(city.Id))
                    {
                        return new NotFoundViewResult("CityNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(city);
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return new NotFoundViewResult("CityNotFound");
            }

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            await _cityRepository.DeleteAsync(city);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CityNotFound()
        {
            return View();
        }

        public IActionResult CityAlreadyExists()
        {
            return View();
        }
    }
}
