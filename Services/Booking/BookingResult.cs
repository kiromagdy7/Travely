namespace Travely.Services.Bookings
{
    public class BookingResult<T>
    {
        public bool Success { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public string? ErrorCode { get; private set; }
        public T? Data { get; private set; }

        public static BookingResult<T> Ok(T data, string message = "") =>
            new BookingResult<T> { Success = true, Data = data, Message = message };

        public static BookingResult<T> Fail(string message, string? errorCode = null) =>
            new BookingResult<T> { Success = false, Message = message, ErrorCode = errorCode };
    }
}