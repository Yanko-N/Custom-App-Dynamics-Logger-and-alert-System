using Domain.Common.List;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILogsRepository
    {
        Task<PaginatedList<CustomLog>> GetLogsAsync(int apiKey, int skip, int take);

    }
}
