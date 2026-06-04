using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAlertRepository
    {
        Task<int?> CreateAlertAsync(int serviceId, string name, string level, string condition, int thresholdValue, int windowSeconds, string? messagePattern, CancellationToken cancellationToken);
        Task<IEnumerable<Alert>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken);
        Task<IEnumerable<Alert>> GetActiveByServiceIdAsync(int serviceId, CancellationToken cancellationToken);
        Task<Alert?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, string name, string level, string condition, int thresholdValue, int windowSeconds, bool isActive, string? messagePattern, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<long> CreateTriggerAsync(int alertId, string details, CancellationToken cancellationToken);
        Task CreateHookEventAsync(int hookId, long triggerId, string payload, int? statusCode, string status, CancellationToken cancellationToken);
    }
}
