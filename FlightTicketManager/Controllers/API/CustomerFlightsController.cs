using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerFlightsController : Controller
    {
        private readonly ITicketRepository _ticketRepository;

        public CustomerFlightsController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        [HttpGet]
        public IActionResult GetCustomerFlights()
        {
            try
            {
                var emailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var userEmail = emailClaim?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    return NotFound("User not found.");
                }

                var tickets = _ticketRepository.GetTicketsByUserEmail(userEmail)
                    .Select(t => new
                    {
                        t.Id,
                        t.PassengerName,
                        t.PassengerId,
                        t.PassengerBirthDate,
                        t.TicketBuyer,
                        t.Seat,
                        t.Price,
                        Flight = new
                        {
                            t.Flight.Id,
                            t.Flight.FlightNumber,
                            t.Flight.DepartureDateTime,
                            FlightDuration = t.Flight.FlightDuration.ToString(),
                            t.Flight.ArrivalTime,
                            Origin = t.Flight.Origin.Name, 
                            Destination = t.Flight.Destination.Name,
                            Aircraft = t.Flight.Aircraft.Data,
                            t.Flight.AvailableSeatsNumber,
                        }
                    })
                    .ToList();

                if (tickets == null || tickets.Count == 0)
                {
                    return NotFound("No future flights found for this user.");
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
