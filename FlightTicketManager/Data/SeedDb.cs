using System;
using System.Linq;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Aircrafts.Any())
            {
                AddAircraft("Airbus319", "TAP");
                AddAircraft("Airbus329", "Ryanair");
                AddAircraft("Airbus339", "EasyJet");
                AddAircraft("Airbus339", "TAP");
                await _context.SaveChangesAsync();
            }
        }

        private void AddAircraft(string description, string airline)
        {
            _context.Aircrafts.Add(new Aircraft
            {
                Description = description,
                Airline = airline,
                Capacity = _random.Next(150, 300),
                IsActive = true
            });
        }
    }
}
