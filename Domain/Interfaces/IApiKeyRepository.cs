using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IApiKeyRepository
    {
        Task<(int Id, string RawKey)?> CreateApiKeyAsync(int accountId, string label, DateTime? expiresAt, CancellationToken cancellationToken);
        Task<IEnumerable<ApiKey>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken);
        Task<ApiKey?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> RevokeAsync(int id, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Hashes the raw key and looks it up. Returns the ApiKey entity if valid and active, null otherwise.
        /// </summary>
        Task<ApiKey?> ValidateApiKeyAsync(string rawKey, CancellationToken cancellationToken);
    }
}
