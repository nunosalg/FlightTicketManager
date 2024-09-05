using System.Collections.Generic;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Models
{
    public class UserRolesViewModel
    {
        public User User { get; set; }


        public IList<string> Roles { get; set; }
    }
}
