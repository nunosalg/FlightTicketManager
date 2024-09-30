using System.Collections.Generic;

namespace FlightTicketManager.Models
{
    public class ChangeSeatViewModel
    {
        public int TicketId { get; set; }

        public int FlightId { get; set; }

        public string CurrentSeat { get; set; }

        public string NewSeat { get; set; }

        public List<string> AvailableSeats { get; set; }
    }
}
