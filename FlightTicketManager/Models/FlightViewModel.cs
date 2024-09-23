using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Models
{
    public class FlightViewModel
    {
        public int Id { get; set; }


        [Display(Name = "Flight Number")]
        public string FlightNumber => Id > 9 ? $"FTM{Id}" : $"FTM0{Id}";


        [Required]
        [Display(Name = "Departure")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DepartureDateTime { get; set; }


        [Required]
        [Display(Name = "Duration")]
        public TimeSpan FlightDuration { get; set; }


        [Required]
        [Display(Name = "Selected origin")]
        public int SelectedOrigin { get; set; }


        public IEnumerable<SelectListItem> Cities { get; set; }


        [Required]
        [Display(Name = "Selected destination")]
        public int SelectedDestination { get; set; }


        [Required]
        [Display(Name = "Selected aircraft")]
        public int SelectedAircraft { get; set; }


        public IEnumerable<SelectListItem> Aircrafts { get; set; }

        public List<string> AvailableSeats { get; set; }


        public User User { get; set; }

        public List<Ticket> TicketsList { get; set; }
    }
}
