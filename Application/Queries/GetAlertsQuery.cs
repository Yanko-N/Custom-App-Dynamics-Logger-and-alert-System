using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetAlertsQuery : IRequest<Result<IEnumerable<Alert>>>
    {
        public int ServiceId { get; init; }

        public GetAlertsQuery(int serviceId) => ServiceId = serviceId;
    }

    public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, Result<IEnumerable<Alert>>>
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ILogger<GetAlertsQueryHandler> _logger;

        public GetAlertsQueryHandler(IAlertRepository alertRepository, ILogger<GetAlertsQueryHandler> logger)
        {
            _alertRepository = alertRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Alert>>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var alerts = await _alertRepository.GetByServiceIdAsync(request.ServiceId, cancellationToken);
                _logger.LogInformation("Fetched alerts for service {ServiceId}", request.ServiceId);
                return Result.Success(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching alerts for service {ServiceId}", request.ServiceId);
                return Result.Failure<IEnumerable<Alert>>(AlertErrors.Unknown);
            }
        }
    }
}
