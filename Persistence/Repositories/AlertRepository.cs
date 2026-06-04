using Domain.Common.Exceptions;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AlertRepository> _logger;

        public AlertRepository(AppDbContext context, ILogger<AlertRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int?> CreateAlertAsync(int serviceId, string name, string level, string condition,
            int thresholdValue, int windowSeconds, string? messagePattern, CancellationToken cancellationToken)
        {
            bool serviceExists = await _context.Services.AnyAsync(s => s.Id == serviceId, cancellationToken);
            if (!serviceExists) return null;

            bool nameConflict = await _context.Alerts
                .AnyAsync(a => a.ServiceId == serviceId && a.Name == name, cancellationToken);

            if (nameConflict)
            {
                throw new AlertNameConflictException(name);
            }

            var alert = new Alert
            {
                ServiceId = serviceId,
                Name = name,
                Level = level,
                Condition = condition,
                ThresholdValue = thresholdValue,
                WindowSeconds = windowSeconds,
                MessagePattern = string.IsNullOrWhiteSpace(messagePattern) ? null : messagePattern.Trim(),
                IsActive = true
            };

            try
            {
                await _context.Alerts.AddAsync(alert, cancellationToken);
                bool saved = await _context.SaveChangesAsync(cancellationToken) > 0;
                return saved ? alert.Id : null;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating alert for service {ServiceId}", serviceId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating alert for service {ServiceId}", serviceId);
                throw;
            }
        }

        public async Task<IEnumerable<Alert>> GetByServiceIdAsync(int serviceId, CancellationToken cancellationToken)
        {
            return await _context.Alerts
                .AsNoTracking()
                .Where(a => a.ServiceId == serviceId)
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Alert>> GetActiveByServiceIdAsync(int serviceId, CancellationToken cancellationToken)
        {
            return await _context.Alerts
                .AsNoTracking()
                .Where(a => a.ServiceId == serviceId && a.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<Alert?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Alerts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateAsync(int id, string name, string level, string condition,
            int thresholdValue, int windowSeconds, bool isActive, string? messagePattern, CancellationToken cancellationToken)
        {
            var alert = await _context.Alerts.FindAsync([id], cancellationToken);
            if (alert == null) return false;

            bool nameConflict = await _context.Alerts
                .AnyAsync(a => a.ServiceId == alert.ServiceId && a.Name == name && a.Id != id, cancellationToken);

            if (nameConflict)
            {
                throw new AlertNameConflictException(name);
            }

            alert.Name = name;
            alert.Level = level;
            alert.Condition = condition;
            alert.ThresholdValue = thresholdValue;
            alert.WindowSeconds = windowSeconds;
            alert.IsActive = isActive;
            alert.MessagePattern = string.IsNullOrWhiteSpace(messagePattern) ? null : messagePattern.Trim();

            try
            {
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error updating alert {AlertId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var alert = await _context.Alerts.FindAsync([id], cancellationToken);
            if (alert == null) return false;

            try
            {
                _context.Alerts.Remove(alert);
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting alert {AlertId}", id);
                return false;
            }
        }

        public async Task<long> CreateTriggerAsync(int alertId, string details, CancellationToken cancellationToken)
        {
            var trigger = new AlertTrigger
            {
                AlertId = alertId,
                FiredAt = DateTime.UtcNow,
                Details = details
            };

            await _context.AlertTriggers.AddAsync(trigger, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return trigger.Id;
        }

        public async Task CreateHookEventAsync(int hookId, long triggerId, string payload, int? statusCode, string status, CancellationToken cancellationToken)
        {
            var ev = new HookEvent
            {
                HookId = hookId,
                AlertTriggerId = triggerId,
                Payload = payload,
                StatusCode = statusCode,
                Status = status,
                Attempts = 1,
                LastAttemptAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _context.HookEvents.AddAsync(ev, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
