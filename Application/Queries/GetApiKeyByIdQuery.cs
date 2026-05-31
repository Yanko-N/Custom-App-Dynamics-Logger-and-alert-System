using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetApiKeyByIdQuery : IRequest<Result<ApiKey>>
    {
        public int Id { get; init; }

        public GetApiKeyByIdQuery(int id) => Id = id;
    }

    public class GetApiKeyByIdQueryHandler : IRequestHandler<GetApiKeyByIdQuery, Result<ApiKey>>
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ILogger<GetApiKeyByIdQueryHandler> _logger;

        public GetApiKeyByIdQueryHandler(IApiKeyRepository apiKeyRepository, ILogger<GetApiKeyByIdQueryHandler> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _logger = logger;
        }

        public async Task<Result<ApiKey>> Handle(GetApiKeyByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var key = await _apiKeyRepository.GetByIdAsync(request.Id, cancellationToken);

                if (key == null)
                {
                    _logger.LogWarning("API key {ApiKeyId} not found", request.Id);
                    return Result.Failure<ApiKey>(ApiKeyErrors.NotFound(request.Id));
                }

                return Result.Success(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching API key {ApiKeyId}", request.Id);
                return Result.Failure<ApiKey>(ApiKeyErrors.Unknown);
            }
        }
    }
}
