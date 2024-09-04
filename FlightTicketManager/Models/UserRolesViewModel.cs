using FlightTicketManager.Data.Entities;
using System.Collections.Generic;

namespace FlightTicketManager.Models
{
    public class UserRolesViewModel
    {
        public User User { get; set; }


        public IList<string> Roles { get; set; }
    }
}
