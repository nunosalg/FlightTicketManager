using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Models
{
    public class UserRoleViewModel
    {
        public string Id { get; set; }


        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        public string Email { get; set; }


        public string Role { get; set; }


        [Display(Name = "Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
