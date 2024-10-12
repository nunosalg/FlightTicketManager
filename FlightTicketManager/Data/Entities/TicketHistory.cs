using System;
using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Data.Entities
{
    public class TicketHistory : IEntity
    {
        public int Id { get; set; }


        public string TicketBuyer { get; set; }


        [Display(Name = "Flight Number")]
        public string FlightNumber { get; set; }


        [Display(Name = "Departure")]
        public DateTime DepartureDateTime { get; set; }


        public string Origin { get; set; }


        public string Destination { get; set; }


        public string OriginAirport { get; set; }


        public string DestinationAirport { get; set; }


        public string Seat { get; set; }


        [Display(Name = "Passenger Name")]
        public string PassengerName { get; set; }


        [Display(Name = "Passenger ID")]
        public string PassengerId { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }


        public string Status { get; set; } // Used, Refunded or Cancelled
    }
}
