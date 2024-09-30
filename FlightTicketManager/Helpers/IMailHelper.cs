using System.Threading.Tasks;

namespace FlightTicketManager.Helpers
{
    public interface IMailHelper
    {
        Task<Response> SendEmailAsync(string to, string subject, string body);

        Task SendCancellationEmailAsync(string userEmail, string flightNumber, decimal ticketPrice);
    }
}
