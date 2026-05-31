using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetApiKeysQuery : IRequest<Result<IEnumerable<ApiKey>>>
    {
        public int AccountId { get; init; }

        public GetApiKeysQuery(int accountId) => AccountId = accountId;
    }

    public class GetApiKeysQueryHandler : IRequestHandler<GetApiKeysQuery, Result<IEnumerable<ApiKey>>>
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ILogger<GetApiKeysQueryHandler> _logger;

        public GetApiKeysQueryHandler(IApiKeyRepository apiKeyRepository, ILogger<GetApiKeysQueryHandler> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<ApiKey>>> Handle(GetApiKeysQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var keys = await _apiKeyRepository.GetByAccountIdAsync(request.AccountId, cancellationToken);
                _logger.LogInformation("Fetched API keys for account {AccountId}", request.AccountId);
                return Result.Success(keys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching API keys for account {AccountId}", request.AccountId);
                return Result.Failure<IEnumerable<ApiKey>>(ApiKeyErrors.Unknown);
            }
        }
    }
}
