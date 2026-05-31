using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IServiceRepository
    {
        Task<int?> CreateServiceAsync(int accountId, string name, string environment, string version, CancellationToken cancellationToken);
        Task<IEnumerable<Service>> GetByAccountIdAsync(int accountId, CancellationToken cancellationToken);
        Task<Service?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, string name, string environment, string version, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
