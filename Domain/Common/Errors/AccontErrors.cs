
namespace Domain.Common.Errors
{
    public static class AccountErrors
    {
        public static Error NameTaken =>
            Error.Conflict("Account.NameTaken", "An account with this name already exists.");

        public static Error InvalidApiKey =>
            Error.Unauthorized("Account.InvalidApiKey", "The provided API key is invalid or expired.");

        public static Error NotFound(int id) =>
            Error.NotFound("Account.NotFound", $"Account with id '{id}' was not found.");
        public static Error Unknown =>
            Error.Unknown("Account.Unknown", "An unknown error occurred while processing the account.");
        public static Error ErrorWhileSaving => 
            Error.Failure("Account.ErrorWhileSaving", "An error occurred while saving the account to the database.");
        public static Error InvalidName =>
            Error.BadRequest("Account.InvalidName", "The account name provided is invalid. It cannot be empty or whitespace.");
    }
}