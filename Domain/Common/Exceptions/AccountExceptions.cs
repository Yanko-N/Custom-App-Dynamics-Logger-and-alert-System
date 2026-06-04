
namespace Domain.Common.Exceptions
{
    public class AccountNameConflictException : Exception
    {
        public AccountNameConflictException(string name)
            : base($"An account with the name '{name}' already exists.")
        {
        }
    }
}