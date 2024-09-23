﻿using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Data.Entities
{
    public class Ticket : IEntity
    {
        public int Id { get; set; }


        [Required]
        public Flight Flight { get; set; }


        [Required]
        public string Seat { get; set; }


        [Required]
        [Display(Name ="Ticket buyer")]
        public User TicketBuyer { get; set; }


        [Required]
        [Display(Name = "Passenger name")]
        public string PassengerName { get; set; }


        [Required]
        [Display(Name = "Passenger identification")]
        public string PassengerId { get; set; }


        [Required]
        [Display(Name = "Passenger birthdate")]
        public DateTime PassengerBirthDate { get; set; }
    }
}