using Microsoft.AspNetCore.Mvc;
using FlightTicketManager.Data;

namespace FlightTicketManager.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : Controller
    {
        private readonly ICityRepository _cityRepository;

        public CitiesController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            return Ok(_cityRepository.GetAll());
        }
    }
}
