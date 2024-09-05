using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using FlightTicketManager.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightTicketManager.Models
{
    public class ChangeUserViewModel
    {
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
        [Display(Name = "Birth Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime BirthDate { get; set; }


        [Display(Name = "Avatar")]
        public IFormFile ImageFile { get; set; }


        [Required]
        public string SelectedRole { get; set; }


        //public string Role { get; set; }


        public IEnumerable<SelectListItem> RolesSelectList { get; set; }
    }
}
