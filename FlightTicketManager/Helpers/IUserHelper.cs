using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}
