using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FlightTicketManager.Data;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Controllers
{
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
                return NotFound();
            }

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Cities/Create
        [Authorize]
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
                    ModelState.AddModelError("", "The city you are trying to add already exists in the database.");
                    return View(city);
                    //return RedirectToAction("CityAlreadyExists");
                }
            }
            return View(city);
        }

        //public IActionResult CityAlreadyExists()
        //{
        //    return View();
        //}

        // GET: Cities/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return NotFound();
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
                        return NotFound();
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
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return NotFound();
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
    }
}
