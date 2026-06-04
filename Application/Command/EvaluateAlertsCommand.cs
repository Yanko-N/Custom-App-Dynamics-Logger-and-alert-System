using Application.Interfaces;
using Domain.Common;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Command
{
    public class EvaluateAlertsCommand : IRequest<Unit>
    {
        public int ServiceId { get; init; }
        public string Level { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public long LogId { get; init; }
    }

    public class EvaluateAlertsCommandHandler : IRequestHandler<EvaluateAlertsCommand, Unit>
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ILogsRepository _logsRepository;
        private readonly IHookRepository _hookRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EvaluateAlertsCommandHandler> _logger;

        public EvaluateAlertsCommandHandler(
            IAlertRepository alertRepository,
            ILogsRepository logsRepository,
            IHookRepository hookRepository,
            IHttpClientFactory httpClientFactory,
            ILogger<EvaluateAlertsCommandHandler> logger)
        {
            _alertRepository = alertRepository;
            _logsRepository = logsRepository;
            _hookRepository = hookRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Unit> Handle(EvaluateAlertsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var alerts = await _alertRepository.GetActiveByServiceIdAsync(request.ServiceId, cancellationToken);

                foreach (var alert in alerts)
                {
                    // Skip if level doesn't match
                    if (!string.Equals(alert.Level, request.Level, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Skip if message pattern set but current message doesn't match
                    if (!string.IsNullOrWhiteSpace(alert.MessagePattern) &&
                        !request.Message.Contains(alert.MessagePattern, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var windowStart = DateTime.UtcNow.AddSeconds(-alert.WindowSeconds);
                    var count = await _logsRepository.CountLogsInWindowAsync(
                        alert.ServiceId, alert.Level, windowStart, alert.MessagePattern, cancellationToken);

                    bool violated;
                    switch (alert.Condition)
                    {
                        case AlertConditions.GreaterThan:
                            violated = count > alert.ThresholdValue;
                            break;
                        case AlertConditions.LessThan:
                            violated = count < alert.ThresholdValue;
                            break;
                        case AlertConditions.Equals_Symbol:
                            violated = count == alert.ThresholdValue;
                            break;
                        default:
                            violated = false;
                            break;
                    }

                    if (!violated)
                    {
                        continue;
                    }

                    _logger.LogWarning("Alert {AlertId} '{AlertName}' triggered - {Level} count={Count} {Cond} {Threshold}",
                        alert.Id, alert.Name, alert.Level, count, alert.Condition, alert.ThresholdValue);

                    var details = $"{alert.Level} count={count} {alert.Condition} {alert.ThresholdValue}" +
                                  (alert.MessagePattern != null ? $" | pattern='{alert.MessagePattern}'" : "");

                    long triggerId = await _alertRepository.CreateTriggerAsync(alert.Id, details, cancellationToken);

                    var hooks = await _hookRepository.GetByServiceIdAsync(alert.ServiceId, cancellationToken);
                    var activeHooks = hooks.Where(h => h.IsActive).ToList();

                    if (activeHooks.Count == 0)
                    {
                        continue;
                    }

                    var payload = new
                    {
                        alertId = alert.Id,
                        alertName = alert.Name,
                        serviceId  = alert.ServiceId,
                        level = alert.Level,
                        condition = alert.Condition,
                        threshold = alert.ThresholdValue,
                        currentCount = count,
                        windowSeconds = alert.WindowSeconds,
                        messagePattern = alert.MessagePattern,
                        triggerId = triggerId,
                        firedAt  = DateTime.UtcNow
                    };

                    var payloadJson = JsonSerializer.Serialize(payload);

                    foreach (var hook in activeHooks)
                    {
                        await FireHookAsync(hook.Id, hook.Url, triggerId, payloadJson, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating alerts for service {ServiceId}", request.ServiceId);
            }

            return Unit.Value;
        }

        private async Task FireHookAsync(int hookId, string url, long triggerId, string payloadJson, CancellationToken cancellationToken)
        {
            int? statusCode = null;
            string status = "Failed";

            try
            {
                var client = _httpClientFactory.CreateClient("AlertHook");
                using var content = new StringContent(payloadJson, System.Text.Encoding.UTF8, "application/json");
                using var response = await client.PostAsync(url, content, cancellationToken);
                statusCode = (int)response.StatusCode;
                status = response.IsSuccessStatusCode ? "Delivered" : "Failed";
                _logger.LogInformation("Hook {HookId} fired → {StatusCode}", hookId, statusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Hook {HookId} delivery failed: {Message}", hookId, ex.Message);
            }

            try
            {
                await _alertRepository.CreateHookEventAsync(hookId, triggerId, payloadJson, statusCode, status, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save HookEvent for hook {HookId}", hookId);
            }
        }
    }

    public struct Unit
    {
        public static readonly Unit Value = default;
    }
}
