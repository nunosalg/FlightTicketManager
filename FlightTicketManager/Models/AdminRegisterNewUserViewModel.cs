using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightTicketManager.Models
{
    public class AdminRegisterNewUserViewModel
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


        [Required]
        [Display(Name = "Identification number")]
        [StringLength(8)]
        public string IdNumber { get; set; }


        [Required]
        public string SelectedRole { get; set; }


        public IEnumerable<SelectListItem> Roles { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
    }
}
