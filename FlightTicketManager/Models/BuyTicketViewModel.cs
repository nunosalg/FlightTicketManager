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
        [Display(Name = "Passenger Name")]
        public string PassengerName { get; set; }


        [Required]
        [Display(Name = "Passenger ID")]
        [StringLength(8)]
        public string PassengerId { get; set; }


        [Required]
        [Display(Name = "Passenger Birthdate")]
        public DateTime PassengerBirthDate { get; set; }


        public List<string> AvailableSeats { get; set; }


        [Display(Name = "Price (€)")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }


        public void SetTicketPrice()
        {
            this.Price = TicketPrice();
        }

        private decimal TicketPrice()
        {
            decimal costPerMinute = 0.5m;
            decimal distanceInMinutes = (decimal)Flight.FlightDuration.TotalMinutes;
            decimal ocupationCost = Flight.TicketsSold * 0.1m;

            decimal ticketPrice = costPerMinute * distanceInMinutes + ocupationCost;

            return ticketPrice;
        }
    }
}
