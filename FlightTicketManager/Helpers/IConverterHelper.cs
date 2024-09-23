using System.Collections.Generic;
using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Models;

namespace FlightTicketManager.Helpers
{
    public interface IConverterHelper
    {
        Aircraft ToAircraft(AircraftViewModel model, string path, bool isNew);

        AircraftViewModel ToAircraftViewModelAsync(Aircraft aircraft);

        Task<Flight> ToFlightAsync(FlightViewModel model, int originId, int destinationId, int aircraftId, User user, List<Ticket> tickets);

        Task<FlightViewModel> ToFlightViewModelAsync(Flight flight, int aircraftId, User user, List<Ticket> tickets);

        Task<Ticket> ToTicketAsync(BuyTicketViewModel model, User user, int flightId);

        TicketConfirmationViewModel ToTicketConfirmationViewModel(Ticket ticket);
    }
}
