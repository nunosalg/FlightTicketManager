using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using FlightTicketManager.Data;

namespace FlightTicketManager.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
