using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Models
{
    public class AircraftViewModel : Aircraft
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
