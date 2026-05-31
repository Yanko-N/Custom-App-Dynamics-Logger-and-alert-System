using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class IngestLogCommand : IRequest<Result<long>>
    {
        public int ServiceId { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public Guid? TraceId { get; set; }
    }

    public class IngestLogCommandHandler : IRequestHandler<IngestLogCommand, Result<long>>
    {
        private readonly ILogsRepository _logsRepository;
        private readonly ILogger<IngestLogCommandHandler> _logger;

        public IngestLogCommandHandler(ILogsRepository logsRepository, ILogger<IngestLogCommandHandler> logger)
        {
            _logsRepository = logsRepository;
            _logger = logger;
        }

        public async Task<Result<long>> Handle(IngestLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return Result.Failure<long>(LogErrors.InvalidMessage);
                }

                var normalizedLevel = request.Level.ToUpperInvariant();

                if (!LogErrors.ValidLevels.Contains(normalizedLevel))
                {
                    return Result.Failure<long>(LogErrors.InvalidLevel);
                }

                var log = new CustomLog
                {
                    ServiceId = request.ServiceId,
                    Timestamp = DateTime.UtcNow,
                    Level = normalizedLevel,
                    TraceId = request.TraceId ?? Guid.NewGuid(),
                    Message = request.Message,
                    StackTrace = request.StackTrace
                };

                var logId = await _logsRepository.IngestLogAsync(log, cancellationToken);

                if (logId == null)
                {
                    return Result.Failure<long>(LogErrors.ErrorWhileSaving);
                }

                return Result.Success(logId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error ingesting log for service {ServiceId}", request.ServiceId);
                return Result.Failure<long>(LogErrors.Unknown);
            }
        }
    }
}
