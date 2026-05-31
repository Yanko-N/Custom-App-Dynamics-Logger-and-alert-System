using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetAlertByIdQuery : IRequest<Result<Alert>>
    {
        public int Id { get; init; }

        public GetAlertByIdQuery(int id) => Id = id;
    }

    public class GetAlertByIdQueryHandler : IRequestHandler<GetAlertByIdQuery, Result<Alert>>
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ILogger<GetAlertByIdQueryHandler> _logger;

        public GetAlertByIdQueryHandler(IAlertRepository alertRepository, ILogger<GetAlertByIdQueryHandler> logger)
        {
            _alertRepository = alertRepository;
            _logger = logger;
        }

        public async Task<Result<Alert>> Handle(GetAlertByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var alert = await _alertRepository.GetByIdAsync(request.Id, cancellationToken);

                if (alert == null)
                {
                    _logger.LogWarning("Alert {AlertId} not found", request.Id);
                    return Result.Failure<Alert>(AlertErrors.NotFound(request.Id));
                }

                return Result.Success(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching alert {AlertId}", request.Id);
                return Result.Failure<Alert>(AlertErrors.Unknown);
            }
        }
    }
}
