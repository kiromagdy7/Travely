namespace Travely.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int? StatusCode { get; set; }

        public string ErrorMessage { get; set; } = "An error occurred while processing your request.";
    }
}
