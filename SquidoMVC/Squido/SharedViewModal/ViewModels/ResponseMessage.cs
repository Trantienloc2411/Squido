namespace SharedViewModal.ViewModels;

public class ResponseMessage<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public T? Data { get; set; }
    
    // Don't expose the full exception details in production
    // Instead, log it server-side and return appropriate error codes
    public string? ExceptionMessage { get; set; }
}