using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Models;

namespace FlightTicketManager.Helpers
{
    public interface IConverterHelper
    {
        Aircraft ToAircraft(AircraftViewModel model, string path, bool isNew);

        AircraftViewModel ToAircraftViewModel(Aircraft aircraft);

        Task<Flight> ToFlightAsync(FlightViewModel model, int originId, int destinationId, int aircraftId, User user);

        Task<FlightViewModel> ToFlightViewModel(Flight flight, int aircraftId, User user);
    }
}
