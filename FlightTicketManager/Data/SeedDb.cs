using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Helpers;

namespace FlightTicketManager.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Employee");
            await _userHelper.CheckRoleAsync("Customer");

            var user = await _userHelper.GetUserByEmailAsync("nunosalgueiro23@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Nuno",
                    LastName = "Salgueiro",
                    Email = "nunosalgueiro23@gmail.com",
                    UserName = "nunosalgueiro23@gmail.com",
                    BirthDate = new DateTime(1990, 10, 24)
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_context.Aircrafts.Any())
            {
                AddAircraft("Airbus319", "TAP", user);
                AddAircraft("Airbus329", "Ryanair", user);
                AddAircraft("Airbus339", "EasyJet", user);
                AddAircraft("Airbus339", "TAP", user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddAircraft(string description, string airline, User user)
        {
            _context.Aircrafts.Add(new Aircraft
            {
                Description = description,
                Airline = airline,
                Capacity = _random.Next(150, 300),
                IsActive = true,
                User = user
            });
        }
    }
}
