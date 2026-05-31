using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories
{
    public class HookRepository : IHookRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HookRepository> _logger;

        public HookRepository(AppDbContext context, ILogger<HookRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int?> CreateHookAsync(int serviceId, string name, string url, string? secret, CancellationToken cancellationToken)
        {
            bool serviceExists = await _context.Services.AnyAsync(s => s.Id == serviceId, cancellationToken);

            if (!serviceExists)
            {
                return null;
            }

            var hook = new Hook
            {
                ServiceId = serviceId,
                Name = name,
                Url = url,
                Secret = secret,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _context.Hooks.AddAsync(hook, cancellationToken);
                bool saved = await _context.SaveChangesAsync(cancellationToken) > 0;
                return saved ? hook.Id : null;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating hook for service {ServiceId}", serviceId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating hook for service {ServiceId}", serviceId);
                throw;
            }
        }

        public async Task<IEnumerable<Hook>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken)
        {
            return await _context.Hooks
                .AsNoTracking()
                .Where(h => h.ServiceId == serviceId)
                .OrderBy(h => h.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Hook?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Hooks
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, string name, string url, string? secret, bool isActive, CancellationToken cancellationToken)
        {
            var hook = await _context.Hooks.FindAsync([id], cancellationToken);

            if (hook == null)
            {
                return false;
            }

            hook.Name = name;
            hook.Url = url;
            hook.Secret = secret;
            hook.IsActive = isActive;

            try
            {
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating hook {HookId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var hook = await _context.Hooks.FindAsync([id], cancellationToken);

            if (hook == null)
            {
                return false;
            }

            try
            {
                _context.Hooks.Remove(hook);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting hook {HookId}", id);
                return false;
            }
        }
    }
}
