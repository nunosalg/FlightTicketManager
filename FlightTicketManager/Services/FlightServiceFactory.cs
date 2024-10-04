using Microsoft.Extensions.DependencyInjection;
using FlightTicketManager.Data.Repositories;

namespace FlightTicketManager.Services
{
    public class FlightServiceFactory : IFlightServiceFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FlightServiceFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IFlightRepository CreateFlightRepository()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                return scope.ServiceProvider.GetRequiredService<IFlightRepository>();
            }
        }

        public IHistoryService CreateHistoryService()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                return scope.ServiceProvider.GetRequiredService<IHistoryService>();
            }
        }
    }
}
