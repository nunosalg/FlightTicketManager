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


        [Required]
        [Display(Name = "Departure")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DepartureDateTime { get; set; }


        [Required]
        [Display(Name = "Duration")]
        public TimeSpan FlightDuration { get; set; }


        [Required]
        [Display(Name = "Selected origin")]
        public string SelectedOrigin { get; set; }


        public IEnumerable<SelectListItem> Cities { get; set; }


        [Required]
        [Display(Name = "Selected destination")]
        public string SelectedDestination { get; set; }


        [Required]
        [Display(Name = "Selected aircraft")]
        public int SelectedAircraft { get; set; }


        public IEnumerable<SelectListItem> Aircrafts { get; set; }

        public List<string> AvailableSeats { get; set; }


        public User User { get; set; }
    }
}
