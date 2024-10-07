using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Models
{
    public class FlightSearchViewModel
    {
        [Display(Name = "Departure Date")]
        public DateTime? DepartureDateTime { get; set; }


        [Display(Name = "Origin City")]
        public int SelectedOrigin { get; set; }


        [Display(Name = "Destination City")]
        public int SelectedDestination { get; set; }


        public IEnumerable<SelectListItem> Cities { get; set; }


        public IEnumerable<Flight> FlightsResults { get; set; }
    }
}
