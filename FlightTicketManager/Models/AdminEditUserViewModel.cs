using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightTicketManager.Models
{
    public class AdminEditUserViewModel
    {
        public string Id { get; set; }


        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }


        [Required]
        public string SelectedRole { get; set; }


        public IEnumerable<SelectListItem> RolesSelectList { get; set; }
    }
}
