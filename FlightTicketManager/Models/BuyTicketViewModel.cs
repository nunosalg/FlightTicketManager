using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Models
{
    public class BuyTicketViewModel
    {
        public int FlightId { get; set; }


        public Flight Flight { get; set; }


        [Required(ErrorMessage = "Please select a seat.")]
        [Display(Name = "Select Seat")]
        public string Seat { get; set; }

        public User Buyer { get; set; }


        [Required]
        [Display(Name = "Passenger name")]
        public string PassengerName { get; set; }


        [Required]
        [Display(Name = "Passenger identification")]
        [StringLength(8)]
        public string PassengerId { get; set; }


        [Required]
        [Display(Name = "Passenger birthdate")]
        public DateTime PassengerBirthDate { get; set; }


        public List<string> AvailableSeats { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

    }
}
