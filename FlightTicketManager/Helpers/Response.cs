﻿namespace FlightTicketManager.Helpers
{
    public class Response
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public object Results { get; set; }
    }
}
