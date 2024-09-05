using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Models;
using System.Security.Claims;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace FlightTicketManager.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<User> GetUserByIdAsync(string userId);

        Task<User> GetCurrentUserAsync(ClaimsPrincipal principal);

        IQueryable<User> GetAllUsers();

        Task<IList<string>> GetUserRolesAsync(User user);
    }
}
