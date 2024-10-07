using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }


        [MinLength(6)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least {2} characters long.")]
        public string Password { get; set; }


        public bool RememberMe { get; set; }    
    }
}
