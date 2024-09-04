using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FlightTicketManager.Helpers;
using Microsoft.AspNetCore.Http;

namespace FlightTicketManager.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [Display(Name = "Birth Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime BirthDate { get; set; }


        [Display(Name = "Avatar")]
        public IFormFile ImageFile { get; set; }

        public List<RoleCheckBox> Roles { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }


        [Required]
        [MinLength(6)]
        public string Password { get; set; }


        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }
    }
}
