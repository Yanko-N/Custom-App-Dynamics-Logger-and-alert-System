using Domain.Common.Exceptions;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        /// <exception cref="AccountAlreadyExistsException">Thrown when an account with the same name already exists.</exception>
        Task<int?> CreateAccountAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<Account>> GetAllAccountsAsync(CancellationToken cancellationToken);
        Task<Account?> GetAccountByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateAccountAsync(int id, string name, bool isActive, CancellationToken cancellationToken);
        Task<bool> DeleteAccountAsync(int id, CancellationToken cancellationToken);
    }
}
