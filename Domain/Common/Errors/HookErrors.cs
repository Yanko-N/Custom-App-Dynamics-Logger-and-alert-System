namespace Domain.Common.Errors
{
    public static class HookErrors
    {
        public static Error NotFound(int id) =>
            Error.NotFound("Hook.NotFound", $"Hook with id '{id}' was not found.");

        public static Error ServiceNotFound(int serviceId) =>
            Error.NotFound("Hook.ServiceNotFound", $"Service with id '{serviceId}' was not found.");

        public static Error InvalidName =>
            Error.BadRequest("Hook.InvalidName", "The hook name is invalid. It cannot be empty or whitespace.");

        public static Error InvalidUrl =>
            Error.BadRequest("Hook.InvalidUrl", "The webhook URL is invalid. It cannot be empty and must be a valid URL.");

        public static Error ErrorWhileSaving =>
            Error.Failure("Hook.ErrorWhileSaving", "An error occurred while saving the hook to the database.");

        public static Error Unknown =>
            Error.Unknown("Hook.Unknown", "An unknown error occurred while processing the hook.");
    }
}
