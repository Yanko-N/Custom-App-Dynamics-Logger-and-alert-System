using Domain.Common.List;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories
{
    public class LogsRepository : ILogsRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LogsRepository> _logger;

        public LogsRepository(AppDbContext context, ILogger<LogsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedList<CustomLog>> GetLogsAsync(int serviceId, int skip, int take)
        {
            var query = _context.Logs
                .AsNoTracking()
                .Where(l => l.ServiceId == serviceId)
                .OrderByDescending(l => l.Timestamp);

            return await PaginatedList<CustomLog>.CreateAsync(query, skip, take);
        }

        public async Task<int> CountLogsInWindowAsync(int serviceId, string level, DateTime from, string? messagePattern, CancellationToken cancellationToken)
        {
            var query = _context.Logs
                .AsNoTracking()
                .Where(l => l.ServiceId == serviceId && l.Level == level.ToUpperInvariant() && l.Timestamp >= from);

            if (!string.IsNullOrWhiteSpace(messagePattern))
                query = query.Where(l => EF.Functions.Like(l.Message, $"%{messagePattern}%"));

            return await query.CountAsync(cancellationToken);
        }

        public async Task<long?> IngestLogAsync(CustomLog log, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Logs.AddAsync(log, cancellationToken);
                bool saved = await _context.SaveChangesAsync(cancellationToken) > 0;
                return saved ? log.Id : null;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error ingesting log for service {ServiceId}", log.ServiceId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error ingesting log for service {ServiceId}", log.ServiceId);
                throw;
            }
        }
    }
}
