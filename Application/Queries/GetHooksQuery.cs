using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetHooksQuery : IRequest<Result<IEnumerable<Hook>>>
    {
        public int ServiceId { get; init; }

        public GetHooksQuery(int serviceId) => ServiceId = serviceId;
    }

    public class GetHooksQueryHandler : IRequestHandler<GetHooksQuery, Result<IEnumerable<Hook>>>
    {
        private readonly IHookRepository _hookRepository;
        private readonly ILogger<GetHooksQueryHandler> _logger;

        public GetHooksQueryHandler(IHookRepository hookRepository, ILogger<GetHooksQueryHandler> logger)
        {
            _hookRepository = hookRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Hook>>> Handle(GetHooksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var hooks = await _hookRepository.GetByServiceIdAsync(request.ServiceId, cancellationToken);
                _logger.LogInformation("Fetched hooks for service {ServiceId}", request.ServiceId);
                return Result.Success(hooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching hooks for service {ServiceId}", request.ServiceId);
                return Result.Failure<IEnumerable<Hook>>(HookErrors.Unknown);
            }
        }
    }
}
