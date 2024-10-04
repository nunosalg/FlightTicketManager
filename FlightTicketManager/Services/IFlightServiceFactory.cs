using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Services
{
    public interface IFlightServiceFactory
    {
        IFlightRepository CreateFlightRepository();

        IHistoryService CreateHistoryService();
    }
}
