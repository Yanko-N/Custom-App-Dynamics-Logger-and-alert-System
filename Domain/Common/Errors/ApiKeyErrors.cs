namespace Domain.Common.Errors
{
    public static class ApiKeyErrors
    {
        public static Error NotFound(int id) =>
            Error.NotFound("ApiKey.NotFound", $"API key with id '{id}' was not found.");

        public static Error AccountNotFound(int accountId) =>
            Error.NotFound("ApiKey.AccountNotFound", $"Account with id '{accountId}' was not found.");

        public static Error AlreadyRevoked =>
            Error.Conflict("ApiKey.AlreadyRevoked", "This API key has already been revoked.");

        public static Error InvalidLabel =>
            Error.BadRequest("ApiKey.InvalidLabel", "The API key label is invalid. It cannot be empty or whitespace.");

        public static Error ErrorWhileSaving =>
            Error.Failure("ApiKey.ErrorWhileSaving", "An error occurred while saving the API key to the database.");

        public static Error Unknown =>
            Error.Unknown("ApiKey.Unknown", "An unknown error occurred while processing the API key.");
    }
}
