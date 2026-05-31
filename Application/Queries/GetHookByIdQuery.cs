using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetHookByIdQuery : IRequest<Result<Hook>>
    {
        public int Id { get; init; }

        public GetHookByIdQuery(int id) => Id = id;
    }

    public class GetHookByIdQueryHandler : IRequestHandler<GetHookByIdQuery, Result<Hook>>
    {
        private readonly IHookRepository _hookRepository;
        private readonly ILogger<GetHookByIdQueryHandler> _logger;

        public GetHookByIdQueryHandler(IHookRepository hookRepository, ILogger<GetHookByIdQueryHandler> logger)
        {
            _hookRepository = hookRepository;
            _logger = logger;
        }

        public async Task<Result<Hook>> Handle(GetHookByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var hook = await _hookRepository.GetByIdAsync(request.Id, cancellationToken);

                if (hook == null)
                {
                    _logger.LogWarning("Hook {HookId} not found", request.Id);
                    return Result.Failure<Hook>(HookErrors.NotFound(request.Id));
                }

                return Result.Success(hook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching hook {HookId}", request.Id);
                return Result.Failure<Hook>(HookErrors.Unknown);
            }
        }
    }
}
