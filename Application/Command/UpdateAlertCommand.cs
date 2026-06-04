using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class UpdateAlertCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int ThresholdValue { get; set; }
        public int WindowSeconds { get; set; }
        public bool IsActive { get; set; }
        public string? MessagePattern { get; set; }
    }

    public class UpdateAlertCommandHandler : IRequestHandler<UpdateAlertCommand, Result<bool>>
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ILogger<UpdateAlertCommandHandler> _logger;

        public UpdateAlertCommandHandler(IAlertRepository alertRepository, ILogger<UpdateAlertCommandHandler> logger)
        {
            _alertRepository = alertRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(UpdateAlertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result.Failure<bool>(AlertErrors.InvalidName);
                }

                var normalizedLevel = request.Level.ToUpperInvariant();

                if (!AlertErrors.ValidLevels.Contains(normalizedLevel))
                {
                    return Result.Failure<bool>(AlertErrors.InvalidLevel);
                }

                if (!AlertErrors.ValidConditions.Contains(request.Condition))
                {
                    return Result.Failure<bool>(AlertErrors.InvalidCondition);
                }

                if (request.ThresholdValue <= 0)
                {
                    return Result.Failure<bool>(AlertErrors.InvalidThreshold);
                }

                if (request.WindowSeconds < 1 || request.WindowSeconds > 86400)
                {
                    return Result.Failure<bool>(AlertErrors.InvalidWindow);
                }

                var success = await _alertRepository.UpdateAsync(
                    request.Id, request.Name, normalizedLevel, request.Condition,
                    request.ThresholdValue, request.WindowSeconds, request.IsActive,
                    request.MessagePattern, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(AlertErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating alert {AlertId}", request.Id);
                return Result.Failure<bool>(AlertErrors.Unknown);
            }
        }
    }
}
