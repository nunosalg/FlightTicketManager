using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Services
{
    public class FlightDepartureService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public FlightDepartureService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Set a timer to execute the CheckFlightsForHistoryAsync method every 15 minutes
            _timer = new Timer(CheckFlightsForHistoryAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void CheckFlightsForHistoryAsync(object state)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var flightRepository = scope.ServiceProvider.GetRequiredService<IFlightRepository>();
                    var historyService = scope.ServiceProvider.GetRequiredService<IHistoryService>();

                    var flightsReadyForHistory = await flightRepository.GetFlightsReadyForHistoryAsync();
                    foreach (var flight in flightsReadyForHistory)
                    {
                        // Save in flight history
                        await historyService.SaveFlightHistoryAsync(flight, "Completed", "Used");
                        // Delete from flights
                        await flightRepository.DeleteAsync(flight);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
