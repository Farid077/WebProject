namespace WebProject.ViewModels
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string ErrorMessage { get; set; } = "Error!";

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
