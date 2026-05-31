using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetServicesQuery : IRequest<Result<IEnumerable<Service>>>
    {
        public int AccountId { get; init; }

        public GetServicesQuery(int accountId) => AccountId = accountId;
    }

    public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, Result<IEnumerable<Service>>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetServicesQueryHandler> _logger;

        public GetServicesQueryHandler(IServiceRepository serviceRepository, ILogger<GetServicesQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Service>>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var services = await _serviceRepository.GetByAccountIdAsync(request.AccountId, cancellationToken);
                _logger.LogInformation("Fetched services for account {AccountId}", request.AccountId);
                return Result.Success(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching services for account {AccountId}", request.AccountId);
                return Result.Failure<IEnumerable<Service>>(ServiceErrors.Unknown);
            }
        }
    }
}
