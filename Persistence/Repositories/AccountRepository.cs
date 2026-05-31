using Domain.Common.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(AppDbContext appContext, ILogger<AccountRepository> logger)
        {
            _context = appContext;
            _logger = logger;
        }

        public async Task<int?> CreateAccountAsync(string name, CancellationToken cancellationToken)
        {
            bool alreadyExists = await _context.Accounts.AnyAsync(a => a.Name == name, cancellationToken);

            if (alreadyExists)
            {
                throw new AccountAlreadyExistsException(name);
            }

            var newAccount = new Account
            {
                Name = name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _context.Accounts.AddAsync(newAccount, cancellationToken);
                bool saved = await _context.SaveChangesAsync(cancellationToken) > 0;
                return saved ? newAccount.Id : null;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating account: {Name}", name);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating account {Name}", name);
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync(CancellationToken cancellationToken)
        {
            return await _context.Accounts
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Account?> GetAccountByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateAccountAsync(int id, string name, bool isActive, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts.FindAsync([id], cancellationToken);

            if (account == null)
            {
                return false;
            }

            bool nameConflict = await _context.Accounts
                .AnyAsync(a => a.Name == name && a.Id != id, cancellationToken);

            if (nameConflict)
            {
                throw new AccountAlreadyExistsException(name);
            }

            account.Name = name;
            account.IsActive = isActive;

            try
            {
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating account {AccountId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync(int id, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts.FindAsync([id], cancellationToken);

            if (account == null)
            {
                return false;
            }

            try
            {
                _context.Accounts.Remove(account);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting account {AccountId}", id);
                return false;
            }
        }
    }
}
