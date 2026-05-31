
namespace Domain.Common.Exceptions
{
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(int accountId)
            : base($"Account with ID {accountId} was not found.")
        {
        }
    }

    public class AccountAlreadyExistsException : Exception
    {
        public AccountAlreadyExistsException(string accountName)
            : base($"An account with the name '{accountName}' already exists.")
        {
        }
    }
}