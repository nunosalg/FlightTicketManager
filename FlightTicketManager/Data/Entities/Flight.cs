using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightTicketManager.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Flight Number")]
        public string FlightNumber => Id > 9 ? $"FTM{Id}" : $"FTM0{Id}";


        [Required]
        [Display(Name = "Departure")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime DepartureDateTime { get; set; }


        [Required]
        [Display(Name = "Duration")]
        public TimeSpan FlightDuration { get; set; }


        [Display(Name = "Arrival")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime ArrivalTime => DepartureDateTime + FlightDuration;


        [Required]
        public string Origin { get; set; }


        [Required]
        public string Destination { get; set; }


        [Required]
        public Aircraft Aircraft { get; set; }


        public string AvailableSeatsJson { get; set; }


        [NotMapped]
        public List<string> AvailableSeats => Aircraft.Seats;


        public int AvailableSeatsNumber => AvailableSeats.Count;


        [NotMapped]
        public List<Ticket> TicketsList { get; set; }


        [Required]
        public User User { get; set; }
    }
}
