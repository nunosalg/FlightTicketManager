using System;
using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Models
{
    public class TicketConfirmationViewModel
    {
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }


        [Display(Name = "Flight number")]
        public string FlightNumber { get; set; }


        [Display(Name = "Origin")]
        public string Origin { get; set; }
        

        [Display(Name = "Destination")]
        public string Destination { get; set; }


        [Display(Name = "Departure")]
        public DateTime DepartureDateTime { get; set; }


        public string Seat { get; set; }


        [Display(Name = "Passenger name")]
        public string PassengerName { get; set; }


        [Display(Name = "Passenger identification")]
        public string PassengerId { get; set; }


        [Display(Name = "Passenger birthdate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime PassengerBirthDate { get; set; }
    }
}
