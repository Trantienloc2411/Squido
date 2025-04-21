using SharedViewModal.ViewModels;

namespace WebApplication1.Helper;

public static class ResponseFactory
{
    public static ResponseMessage<T> Success<T>(T data, string message = "Operation successful")
    {
        return new ResponseMessage<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static ResponseMessage<T> Error<T>(string message, string errorCode = null, T data = default)
    {
        return new ResponseMessage<T>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode,
            Data = data
        };
    }
    
    public static ResponseMessage<T> Exception<T>(Exception ex)
    {
        // Log the full exception details server-side
        // Only return safe message to client
        return new ResponseMessage<T>
        {
            IsSuccess = false,
            Message = "An unexpected error occurred",
            ErrorCode = "INTERNAL_ERROR",
            ExceptionMessage = ex.Message // Consider if you want to expose this in production
        };
    }
}