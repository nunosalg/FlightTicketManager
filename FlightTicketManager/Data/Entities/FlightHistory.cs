using System;
using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Data.Entities
{
    public class FlightHistory : IEntity
    {
        public int Id { get; set; }


        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; }


        [Display(Name = "Departure")]
        public DateTime DepartureDateTime { get; set; }


        public TimeSpan Duration { get; set; }


        public string Origin { get; set; }


        public string Destination { get; set; }


        [Display(Name = "Aircraft Data")]
        public string AircraftModelId { get; set; }


        public string Status { get; set; }  // Completed or Cancelled
    }
}
