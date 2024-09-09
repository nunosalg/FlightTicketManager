using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
