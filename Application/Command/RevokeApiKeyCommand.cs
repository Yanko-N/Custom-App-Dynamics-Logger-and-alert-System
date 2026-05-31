using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class RevokeApiKeyCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }

        public RevokeApiKeyCommand(int id) => Id = id;
    }

    public class RevokeApiKeyCommandHandler : IRequestHandler<RevokeApiKeyCommand, Result<bool>>
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ILogger<RevokeApiKeyCommandHandler> _logger;

        public RevokeApiKeyCommandHandler(IApiKeyRepository apiKeyRepository, ILogger<RevokeApiKeyCommandHandler> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(RevokeApiKeyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var key = await _apiKeyRepository.GetByIdAsync(request.Id, cancellationToken);

                if (key == null)
                {
                    return Result.Failure<bool>(ApiKeyErrors.NotFound(request.Id));
                }

                if (!key.IsActive)
                {
                    return Result.Failure<bool>(ApiKeyErrors.AlreadyRevoked);
                }

                var success = await _apiKeyRepository.RevokeAsync(request.Id, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(ApiKeyErrors.ErrorWhileSaving);
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error revoking API key {ApiKeyId}", request.Id);
                return Result.Failure<bool>(ApiKeyErrors.Unknown);
            }
        }
    }
}
