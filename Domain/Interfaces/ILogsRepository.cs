using Domain.Common.List;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILogsRepository
    {
        Task<PaginatedList<CustomLog>> GetLogsAsync(int serviceId, int skip, int take);
        Task<long?> IngestLogAsync(CustomLog log, CancellationToken cancellationToken);
        Task<int> CountLogsInWindowAsync(int serviceId, string level, DateTime from, string? messagePattern, CancellationToken cancellationToken);
    }
}
