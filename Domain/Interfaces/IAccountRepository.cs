using Domain.Common.Exceptions;
namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Creates a new Account for the Username Given and returns the Account ID if successful, or null if an error occurs.
        /// </summary>
        /// <param name="name">Name to be created with</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="AccountAlreadyExistsException">Thrown when an account with the same name already exists.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
        /// <returns>Account Id or Null</returns>
        Task<int?> CreateAccountAsync(string name,CancellationToken cancellationToken);
    }
}
