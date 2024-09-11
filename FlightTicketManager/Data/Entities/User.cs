using System;
using Microsoft.AspNetCore.Identity;

namespace FlightTicketManager.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string IdNumber { get; set; }

        public string AvatarUrl { get; set; }

        public string AvatarFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(AvatarUrl))
                {
                    return "~/images/defaultavatar.png";
                }

                return $"https://localhost:44306{AvatarUrl.Substring(1)}";
            }
        }
    }
}
