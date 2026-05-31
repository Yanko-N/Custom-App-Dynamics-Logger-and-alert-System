namespace Domain.Common.Errors
{
    public static class AlertErrors
    {
        public static readonly string[] ValidLevels = ["DEBUG", "INFO", "WARN", "ERROR", "FATAL"];
        public static readonly string[] ValidConditions = ["GreaterThan", "LessThan", "Equals"];

        public static Error NotFound(int id) =>
            Error.NotFound("Alert.NotFound", $"Alert with id '{id}' was not found.");

        public static Error ServiceNotFound(int serviceId) =>
            Error.NotFound("Alert.ServiceNotFound", $"Service with id '{serviceId}' was not found.");

        public static Error InvalidName =>
            Error.BadRequest("Alert.InvalidName", "The alert name is invalid. It cannot be empty or whitespace.");

        public static Error InvalidLevel =>
            Error.BadRequest("Alert.InvalidLevel", "The log level is invalid. Valid values are: DEBUG, INFO, WARN, ERROR, FATAL.");

        public static Error InvalidCondition =>
            Error.BadRequest("Alert.InvalidCondition", "The alert condition is invalid. Valid values are: GreaterThan, LessThan, Equals.");

        public static Error InvalidThreshold =>
            Error.BadRequest("Alert.InvalidThreshold", "The threshold value must be greater than zero.");

        public static Error InvalidWindow =>
            Error.BadRequest("Alert.InvalidWindow", "The window duration must be between 1 and 86400 seconds.");

        public static Error ErrorWhileSaving =>
            Error.Failure("Alert.ErrorWhileSaving", "An error occurred while saving the alert to the database.");

        public static Error Unknown =>
            Error.Unknown("Alert.Unknown", "An unknown error occurred while processing the alert.");
    }
}
