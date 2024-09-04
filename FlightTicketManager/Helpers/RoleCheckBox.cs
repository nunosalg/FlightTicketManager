using System.ComponentModel.DataAnnotations;

namespace FlightTicketManager.Helpers
{
    public class RoleCheckBox
    {
        [Display(Name = "Role")]
        public string RoleName { get; set; }


        [Display(Name = "Is Selected")]
        public bool IsSelected { get; set; }
    }
}
