using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Models;

namespace FlightTicketManager.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }
}
