namespace FlightTicketManager.Helpers
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }

        public string Error { get; set; }

        public ValidationResult()
        {
            IsValid = true;
        }

        public void AddError(string message)
        {
            IsValid = false;
            Error = message;
        }
    }
}
