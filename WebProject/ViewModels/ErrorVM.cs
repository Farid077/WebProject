namespace WebProject.ViewModels
{
    public class ErrorVM
    {
        public string? RequestId { get; set; }
        public string ErrorMessage { get; set; } = "Error!";

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
