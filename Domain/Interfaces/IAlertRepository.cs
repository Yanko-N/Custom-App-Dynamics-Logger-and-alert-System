using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAlertRepository
    {
        Task<int?> CreateAlertAsync(int serviceId, string name, string level, string condition, int thresholdValue, int windowSeconds, CancellationToken cancellationToken);
        Task<IEnumerable<Alert>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken);
        Task<Alert?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, string name, string level, string condition, int thresholdValue, int windowSeconds, bool isActive, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
