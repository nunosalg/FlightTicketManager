using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Helpers;

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
        public async Task<IActionResult> Create(Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                //TODO: Modificar para o user que estiver logado
                aircraft.User = await _userHelper.GetUserByEmailAsync("nunosalgueiro23@gmail.com");

                await _aircraftRepository.CreateAsync(aircraft);
                return RedirectToAction(nameof(Index));
            }
            return View(aircraft);
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
            return View(aircraft);
        }

        // POST: Aircrafts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Aircraft aircraft)
        {
            if (id != aircraft.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //TODO: Modificar para o user que estiver logado
                    aircraft.User = await _userHelper.GetUserByEmailAsync("nunosalgueiro23@gmail.com");
                    await _aircraftRepository.UpdateAsync(aircraft);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await _aircraftRepository.ExistAsync(aircraft.Id))
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
            return View(aircraft);
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
