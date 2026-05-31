using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Domain.Common.Exceptions;

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

        public async Task<int?> CreateAccountAsync(string name,CancellationToken cancellationToken)
        {
            bool alreadyExists = await _context.Accounts.AnyAsync(a => a.Name == name);

            if (alreadyExists)
            {
                throw new AccountAlreadyExistsException(name);
            }

            Account newAccount = new Account
            {
                Name = name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            bool result = false;
            try
            {
                await _context.Accounts.AddAsync(newAccount);
                result = await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while creating account: {AccountName}", name);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating account {AccountName}", name);
                throw;
            }

            return result ? newAccount.Id : null;
        }
    }
}
