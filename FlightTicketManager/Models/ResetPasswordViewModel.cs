using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }


        [Required]
        public string Token { get; set; }


        [Required]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }


        [Required]
        [Compare("NewPassword")]
        public string Confirm { get; set; }
    }
}
