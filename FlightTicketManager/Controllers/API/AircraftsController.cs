using Microsoft.AspNetCore.Mvc;
using FlightTicketManager.Data;

namespace FlightTicketManager.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftsController : Controller
    {
        private readonly IAircraftRepository _aircraftRepository;

        public AircraftsController(IAircraftRepository aircraftRepository)
        {
            _aircraftRepository = aircraftRepository;
        }

        [HttpGet]
        public IActionResult GetAircrafts()
        {
            return Ok(_aircraftRepository.GetAllWithUsers());
        }
    }
}
