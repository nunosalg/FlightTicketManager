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
        [Display(Name ="Ticket Buyer")]
        public User TicketBuyer { get; set; }


        [Required]
        [Display(Name = "Passenger Name")]
        public string PassengerName { get; set; }


        [Required]
        [Display(Name = "Passenger Identification")]
        public string PassengerId { get; set; }


        [Required]
        [Display(Name = "Passenger birthdate")]
        public DateTime PassengerBirthDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }
    }
}