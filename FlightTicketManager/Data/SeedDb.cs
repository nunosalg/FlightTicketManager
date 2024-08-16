using System;
using System.Linq;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Helpers;
using Microsoft.AspNetCore.Identity;

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
            await _context.Database.EnsureCreatedAsync();

            var user = await _userHelper.GetUserByEmailAsync("nunosalgueiro23@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Nuno",
                    LastName = "Salgueiro",
                    Email = "nunosalgueiro23@gmail.com",
                    UserName = "nunosalgueiro23@gmail.com",
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
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
