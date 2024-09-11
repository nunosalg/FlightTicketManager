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
        public User Passenger { get; set; }
    }
}