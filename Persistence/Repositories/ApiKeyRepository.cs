using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Persistence.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ApiKeyRepository> _logger;

        public ApiKeyRepository(AppDbContext context, ILogger<ApiKeyRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(int Id, string RawKey)?> CreateApiKeyAsync(int accountId, string label, DateTime? expiresAt, CancellationToken cancellationToken)
        {
            bool accountExists = await _context.Accounts.AnyAsync(a => a.Id == accountId, cancellationToken);

            if (!accountExists)
            {
                return null;
            }

            var rawKeyBytes = RandomNumberGenerator.GetBytes(32);
            var rawKey = "aldl_" + Convert.ToHexString(rawKeyBytes).ToLower();
            var keyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey))).ToLower();

            var apiKey = new ApiKey
            {
                AccountId = accountId,
                KeyHash = keyHash,
                Label = label,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            try
            {
                await _context.ApiKeys.AddAsync(apiKey, cancellationToken);
                bool saved = await _context.SaveChangesAsync(cancellationToken) > 0;
                return saved ? (apiKey.Id, rawKey) : null;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating API key for account {AccountId}", accountId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating API key for account {AccountId}", accountId);
                throw;
            }
        }

        public async Task<IEnumerable<ApiKey>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken)
        {
            return await _context.ApiKeys
                .AsNoTracking()
                .Where(k => k.AccountId == accountId)
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<ApiKey?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.ApiKeys
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
        }

        public async Task<bool> RevokeAsync(int id, CancellationToken cancellationToken)
        {
            var key = await _context.ApiKeys.FindAsync([id], cancellationToken);

            if (key == null)
            {
                return false;
            }

            key.IsActive = false;

            try
            {
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error revoking API key {ApiKeyId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var key = await _context.ApiKeys.FindAsync([id], cancellationToken);

            if (key == null)
            {
                return false;
            }

            try
            {
                _context.ApiKeys.Remove(key);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting API key {ApiKeyId}", id);
                return false;
            }
        }

        public async Task<ApiKey?> ValidateApiKeyAsync(string rawKey, CancellationToken cancellationToken)
        {
            var keyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawKey))).ToLower();

            var apiKey = await _context.ApiKeys
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.KeyHash == keyHash, cancellationToken);

            if (apiKey == null || !apiKey.IsActive)
            {
                return null;
            }

            if (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt.Value < DateTime.UtcNow)
            {
                return null;
            }

            return apiKey;
        }
    }
}
