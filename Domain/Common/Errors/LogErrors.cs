namespace Domain.Common.Errors
{
    public static class LogErrors
    {
        public static readonly string[] ValidLevels = ["DEBUG", "INFO", "WARN", "ERROR", "FATAL"];

        public static Error ServiceNotFound(int serviceId) =>
            Error.NotFound("Log.ServiceNotFound", $"Service with id '{serviceId}' was not found.");

        public static Error InvalidLevel =>
            Error.BadRequest("Log.InvalidLevel", "The log level is invalid. Valid values are: DEBUG, INFO, WARN, ERROR, FATAL.");

        public static Error InvalidMessage =>
            Error.BadRequest("Log.InvalidMessage", "The log message cannot be empty or whitespace.");

        public static Error ErrorWhileSaving =>
            Error.Failure("Log.ErrorWhileSaving", "An error occurred while saving the log to the database.");

        public static Error Unknown =>
            Error.Unknown("Log.Unknown", "An unknown error occurred while processing the log.");
    }
}
