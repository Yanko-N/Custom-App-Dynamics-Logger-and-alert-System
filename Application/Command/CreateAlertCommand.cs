using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class CreateAlertCommand : IRequest<Result<int>>
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int ThresholdValue { get; set; }
        public int WindowSeconds { get; set; } = 60;
        public string? MessagePattern { get; set; }
    }

    public class CreateAlertCommandHandler : IRequestHandler<CreateAlertCommand, Result<int>>
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ILogger<CreateAlertCommandHandler> _logger;

        public CreateAlertCommandHandler(IAlertRepository alertRepository, ILogger<CreateAlertCommandHandler> logger)
        {
            _alertRepository = alertRepository;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result.Failure<int>(AlertErrors.InvalidName);
                }

                var normalizedLevel = request.Level.ToUpperInvariant();

                if (!AlertErrors.ValidLevels.Contains(normalizedLevel))
                {
                    return Result.Failure<int>(AlertErrors.InvalidLevel);
                }

                if (!AlertErrors.ValidConditions.Contains(request.Condition))
                {
                    return Result.Failure<int>(AlertErrors.InvalidCondition);
                }

                if (request.ThresholdValue <= 0)
                {
                    return Result.Failure<int>(AlertErrors.InvalidThreshold);
                }

                if (request.WindowSeconds < 1 || request.WindowSeconds > 86400)
                {
                    return Result.Failure<int>(AlertErrors.InvalidWindow);
                }

                var alertId = await _alertRepository.CreateAlertAsync(
                    request.ServiceId, request.Name, normalizedLevel, request.Condition,
                    request.ThresholdValue, request.WindowSeconds, request.MessagePattern, cancellationToken);

                if (alertId == null)
                {
                    return Result.Failure<int>(AlertErrors.ErrorWhileSaving);
                }

                return Result.Success(alertId.Value);
            }
            catch (Domain.Common.Exceptions.AlertNameConflictException)
            {
                return Result.Failure<int>(AlertErrors.NameTaken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating alert for service {ServiceId}", request.ServiceId);
                return Result.Failure<int>(AlertErrors.Unknown);
            }
        }
    }
}
