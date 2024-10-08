﻿using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
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

        IQueryable<IdentityRole> GetAllRoles();

        Task RemoveRolesFromUserAsync(User user, IEnumerable<string> roles);

        Task<IdentityResult> DeleteUser(User user);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task<IQueryable> GetAllUsersExceptAdminsAsync();

        //Task<UserRoleViewModel> GetUserByIdIncludeRoleAsync(string id);

        Task<string> GetUserRoleIdAsync(string userId);

        Task<string> GetRoleNameAsync(string roleId);
    }
}
