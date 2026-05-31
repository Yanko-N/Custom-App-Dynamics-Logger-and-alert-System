namespace Domain.Common.Errors
{
    public static class ServiceErrors
    {
        public static Error NotFound(int id) =>
            Error.NotFound("Service.NotFound", $"Service with id '{id}' was not found.");

        public static Error AccountNotFound(int accountId) =>
            Error.NotFound("Service.AccountNotFound", $"Account with id '{accountId}' was not found.");

        public static Error NameTaken =>
            Error.Conflict("Service.NameTaken", "A service with this name already exists for this account.");

        public static Error InvalidName =>
            Error.BadRequest("Service.InvalidName", "The service name is invalid. It cannot be empty or whitespace.");

        public static Error ErrorWhileSaving =>
            Error.Failure("Service.ErrorWhileSaving", "An error occurred while saving the service to the database.");

        public static Error Unknown =>
            Error.Unknown("Service.Unknown", "An unknown error occurred while processing the service.");
    }
}
