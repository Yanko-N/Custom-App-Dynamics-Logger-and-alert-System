
namespace Domain.Common.Errors
{
    public static class CustomLogsErrors
    {

        public static Error UnknownError =>
            Error.Unknown("CustomLogs.UnknownError", "An unknown error occurred.");
        public static Error InvalidPagination =>
            Error.Validation("CustomLogs.InvalidPagination", "Pagination parameters are invalid. 'Take' must be between 1 and 100, and 'Skip' cannot be negative.");
        public static Error NotFound => 
            Error.NotFound("CustomLogs.NotFound", "No logs found for the given API key.");

    }
}
