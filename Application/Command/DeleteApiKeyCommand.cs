using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class DeleteApiKeyCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }

        public DeleteApiKeyCommand(int id) => Id = id;
    }

    public class DeleteApiKeyCommandHandler : IRequestHandler<DeleteApiKeyCommand, Result<bool>>
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ILogger<DeleteApiKeyCommandHandler> _logger;

        public DeleteApiKeyCommandHandler(IApiKeyRepository apiKeyRepository, ILogger<DeleteApiKeyCommandHandler> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteApiKeyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _apiKeyRepository.DeleteAsync(request.Id, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(ApiKeyErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting API key {ApiKeyId}", request.Id);
                return Result.Failure<bool>(ApiKeyErrors.Unknown);
            }
        }
    }
}
