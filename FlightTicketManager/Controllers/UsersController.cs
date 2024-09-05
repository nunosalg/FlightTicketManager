using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Helpers;
using FlightTicketManager.Models;

namespace FlightTicketManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            IUserHelper userHelper,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userHelper = userHelper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = _userHelper.GetAllUsers();
            var userList = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var role = (await _userHelper.GetUserRolesAsync(user)).FirstOrDefault();


                userList.Add(new UserRoleViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = role 
                });
            }

            return View(userList);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            var model = new AdminRegisterNewUserViewModel
            {
                Roles = _roleManager.Roles.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name,
                }).ToList()
            };

            return View(model);
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminRegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = model.Username,
                        Email = model.Username,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        BirthDate = model.BirthDate
                    };

                    string password = "123456";
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);

                        return RedirectToAction(nameof(Index));
                    }

                }
                ModelState.AddModelError("", "Failed to create user.");
            }

            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var userRole = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles;

            var model = new AdminEditUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                SelectedRole = userRole.FirstOrDefault(), 

                RolesSelectList = allRoles.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name,
                    Selected = userRole.Contains(role.Name)
                }).ToList()
            };

            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminEditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Username); 
                if (user == null)
                {
                    return new NotFoundViewResult("UserNotFound");
                }

                user.UserName = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update user.");
                    return View(model);
                }

                // Remove all roles from the user
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                }

                // Add the new role if selected
                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(user);
        }

        public IActionResult UserNotFound()
        {
            return View();
        }
    }
}

