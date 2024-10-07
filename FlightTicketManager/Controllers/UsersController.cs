using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Helpers;
using FlightTicketManager.Models;
using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;

        public UsersController(
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IFlightRepository flightRepository,
            ITicketRepository ticketRepository)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _userHelper.GetAllUsersExceptAdminsAsync();
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
                Roles = _userHelper.GetAllRoles().Select(role => new SelectListItem
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
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        BirthDate = model.BirthDate,
                    };

                    string password = new Random().ToString();
                    var result = await _userHelper.AddUserAsync(user, password);

                    if (result.Succeeded)
                    {
                        await _userHelper.AddUserToRoleAsync(user, model.SelectedRole);
                        var userToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        await _userHelper.ConfirmEmailAsync(user, userToken);

                        var resetToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                        string tokenLink = Url.Action("ConfirmEmailChangePassword", "Account", new
                        {
                            userid = user.Id,
                            token = resetToken
                        }, protocol: HttpContext.Request.Scheme);

                        Response response = await _mailHelper.SendEmailAsync(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"plase click in this link:</br></br><a href = \"{tokenLink}\">Click here to confirm your  email and change your password</a>");

                        if (response.IsSuccess)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    
                }

                if (user != null)
                {
                    ModelState.AddModelError(model.Username, "Email already exists in database.");
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

            var user = await _userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var userRole = await _userHelper.GetUserRolesAsync(user);
            var allRoles = _userHelper.GetAllRoles();

            var model = new AdminEditUserViewModel
            {
                Id = id,
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
                var user = await _userHelper.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return new NotFoundViewResult("UserNotFound");
                }

                user.UserName = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Username;

                var result = await _userHelper.UpdateUserAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update user.");
                    return View(model);
                }

                // Remove all roles from the user
                var userRoles = await _userHelper.GetUserRolesAsync(user);
                if (userRoles.Any())
                {
                    await _userHelper.RemoveRolesFromUserAsync(user, userRoles);
                }

                // Add the new role if selected
                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    await _userHelper.AddUserToRoleAsync(user, model.SelectedRole);
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

            var user = await _userHelper.GetUserByIdAsync(id);
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
            var user = await _userHelper.GetUserByIdAsync(id);

            var hasFlights = await _flightRepository.HasFlightsByUserAsync(id); 

            if (hasFlights)
            {
                ModelState.AddModelError(string.Empty, "This user cannot be deleted because the user has created one or more flights.");
                return View(user); 
            }

            var hasTickets = await _ticketRepository.HasTicketsByUserAsync(id);

            if (hasTickets)
            {
                ModelState.AddModelError(string.Empty, "This user cannot be deleted because the user has one or more tickets bought.");
                return View(user);
            }


            if (user != null)
            {
                var result = await _userHelper.DeleteUserAsync(user);
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

