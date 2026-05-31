using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IHookRepository
    {
        Task<int?> CreateHookAsync(int serviceId, string name, string url, string? secret, CancellationToken cancellationToken);
        Task<IEnumerable<Hook>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken);
        Task<Hook?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, string name, string url, string? secret, bool isActive, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
