using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public record ApiKeyCreatedResult(int Id, string RawKey, string Label);

    public class CreateApiKeyCommand : IRequest<Result<ApiKeyCreatedResult>>
    {
        public int AccountId { get; set; }
        public string Label { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }

    public class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, Result<ApiKeyCreatedResult>>
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ILogger<CreateApiKeyCommandHandler> _logger;

        public CreateApiKeyCommandHandler(IApiKeyRepository apiKeyRepository, ILogger<CreateApiKeyCommandHandler> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _logger = logger;
        }

        public async Task<Result<ApiKeyCreatedResult>> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Label))
                {
                    return Result.Failure<ApiKeyCreatedResult>(ApiKeyErrors.InvalidLabel);
                }

                var created = await _apiKeyRepository.CreateApiKeyAsync(
                    request.AccountId, request.Label, request.ExpiresAt, cancellationToken);

                if (created == null)
                {
                    return Result.Failure<ApiKeyCreatedResult>(ApiKeyErrors.ErrorWhileSaving);
                }

                return Result.Success(new ApiKeyCreatedResult(created.Value.Id, created.Value.RawKey, request.Label));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating API key for account {AccountId}", request.AccountId);
                return Result.Failure<ApiKeyCreatedResult>(ApiKeyErrors.Unknown);
            }
        }
    }
}
