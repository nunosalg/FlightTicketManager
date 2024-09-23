﻿using System;
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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DepartureDateTime { get; set; }


        [Required]
        [Display(Name = "Duration")]
        public TimeSpan FlightDuration { get; set; }


        [Display(Name = "Arrival")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime ArrivalTime => DepartureDateTime + FlightDuration;


        [Required]
        public City Origin { get; set; }


        [Required]
        public City Destination { get; set; }


        [Required]
        public Aircraft Aircraft { get; set; }


        public List<string> AvailableSeats { get; set; } 


        [Display(Name = "Available seats")]
        public int AvailableSeatsNumber => AvailableSeats.Count;


        [NotMapped]
        public List<Ticket> TicketsList { get; set; } = new List<Ticket>();


        [Required]
        public User User { get; set; }


        /// <summary>
        /// Initializes the available seats according to the aircraft list of seats
        /// </summary>
        public void InitializeAvailableSeats()
        {
            if (Aircraft != null)
            {
                AvailableSeats = new List<string>(Aircraft.Seats);
            }
        }
    }
}
