using Domain.Common.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ServiceRepository> _logger;

        public ServiceRepository(AppDbContext context, ILogger<ServiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int?> CreateServiceAsync(int accountId, string name, string environment, string version, CancellationToken cancellationToken)
        {
            bool accountExists = await _context.Accounts.AnyAsync(a => a.Id == accountId, cancellationToken);

            if (!accountExists)
            {
                return null;
            }

            bool nameConflict = await _context.Services
                .AnyAsync(s => s.AccountId == accountId && s.Name == name, cancellationToken);

            if (nameConflict)
            {
                throw new ServiceNameConflictException(name);
            }

            var service = new Service
            {
                AccountId = accountId,
                Name = name,
                Environment = environment,
                Version = version,
                RegisteredAt = DateTime.UtcNow
            };

            try
            {
                await _context.Services.AddAsync(service, cancellationToken);
                bool saved = await _context.SaveChangesAsync(cancellationToken) > 0;
                return saved ? service.Id : null;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating service {Name} for account {AccountId}", name, accountId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating service {Name}", name);
                throw;
            }
        }

        public async Task<IEnumerable<Service>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken)
        {
            return await _context.Services
                .AsNoTracking()
                .Where(s => s.AccountId == accountId)
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Service?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, string name, string environment, string version, CancellationToken cancellationToken)
        {
            var service = await _context.Services.FindAsync([id], cancellationToken);

            if (service == null)
            {
                return false;
            }

            bool nameConflict = await _context.Services
                .AnyAsync(s => s.AccountId == service.AccountId && s.Name == name && s.Id != id, cancellationToken);

            if (nameConflict)
            {
                throw new ServiceNameConflictException(name);
            }

            service.Name = name;
            service.Environment = environment;
            service.Version = version;

            try
            {
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating service {ServiceId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var service = await _context.Services.FindAsync([id], cancellationToken);

            if (service == null)
            {
                return false;
            }

            try
            {
                _context.Services.Remove(service);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting service {ServiceId}", id);
                return false;
            }
        }
    }
}
