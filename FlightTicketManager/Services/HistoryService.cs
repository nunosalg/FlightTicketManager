using System.Threading.Tasks;
using FlightTicketManager.Data.Entities;
using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IFlightHistoryRepository _flightHistoryRepository;
        private readonly ITicketHistoryRepository _ticketHistoryRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly ITicketRepository _ticketRepository;

        public HistoryService(
            IFlightHistoryRepository flightHistoryRepository,
            ITicketHistoryRepository ticketHistoryRepository,
            IFlightRepository flightRepository,
            ITicketRepository ticketRepository)
        {
            _flightHistoryRepository = flightHistoryRepository;
            _ticketHistoryRepository = ticketHistoryRepository;
            _flightRepository = flightRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task SaveFlightHistoryAsync(Flight flight, string flightStatus, string ticketStatus)
        {
            flight = await _flightRepository.GetByIdWithUsersAircraftsAndCitiesAsync(flight.Id);
            // Save flight history
            var flightHistory = new FlightHistory
            {
                FlightNumber = flight.FlightNumber,
                DepartureDateTime = flight.DepartureDateTime,
                Duration = flight.FlightDuration,
                Origin = flight.Origin.Name,
                OriginAirport = flight.OriginAirport,
                Destination = flight.Destination.Name,
                DestinationAirport = flight.DestinationAirport,
                AircraftModelId = flight.Aircraft.ModelId,
                Status = flightStatus
            };
            await _flightHistoryRepository.CreateAsync(flightHistory);

            // Save tickets history
            foreach (var ticket in flight.TicketsList)
            {
                var ticketHistory = new TicketHistory
                {
                    FlightNumber = flight.FlightNumber,
                    TicketBuyer = ticket.TicketBuyer.Email,
                    DepartureDateTime = flight.DepartureDateTime,
                    Origin = flight.Origin.Name,
                    OriginAirport = flight.OriginAirport,
                    Destination = flight.Destination.Name,
                    DestinationAirport = flight.DestinationAirport,
                    Seat = ticket.Seat,
                    PassengerName = ticket.PassengerName,
                    PassengerId = ticket.PassengerId,
                    Price = ticket.Price,
                    Status = ticketStatus
                };
                await _ticketHistoryRepository.CreateAsync(ticketHistory);
            }
        }

        public async Task SaveTicketHistoryAsync(Ticket ticket, string ticketStatus)
        {
            ticket = await _ticketRepository.GetByIdWithFlightDetailsAsync(ticket.Id);

            var ticketHistory = new TicketHistory
            {
                FlightNumber = ticket.Flight.FlightNumber,
                TicketBuyer = ticket.TicketBuyer.Email,
                DepartureDateTime = ticket.Flight.DepartureDateTime,
                Origin = ticket.Flight.Origin.Name,
                OriginAirport = ticket.Flight.OriginAirport,
                Destination = ticket.Flight.Destination.Name,
                DestinationAirport = ticket.Flight.DestinationAirport,
                Seat = ticket.Seat,
                PassengerName = ticket.PassengerName,
                PassengerId = ticket.PassengerId,
                Price = ticket.Price,
                Status = ticketStatus
            };

            await _ticketHistoryRepository.CreateAsync(ticketHistory);
        }
    }
}
