using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Data.Entities
{
    public class Aircraft : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Airline { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
