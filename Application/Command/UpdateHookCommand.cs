using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class UpdateHookCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Secret { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateHookCommandHandler : IRequestHandler<UpdateHookCommand, Result<bool>>
    {
        private readonly IHookRepository _hookRepository;
        private readonly ILogger<UpdateHookCommandHandler> _logger;

        public UpdateHookCommandHandler(IHookRepository hookRepository, ILogger<UpdateHookCommandHandler> logger)
        {
            _hookRepository = hookRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(UpdateHookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result.Failure<bool>(HookErrors.InvalidName);
                }

                if (string.IsNullOrWhiteSpace(request.Url) || !Uri.TryCreate(request.Url, UriKind.Absolute, out _))
                {
                    return Result.Failure<bool>(HookErrors.InvalidUrl);
                }

                var success = await _hookRepository.UpdateAsync(
                    request.Id, request.Name, request.Url, request.Secret, request.IsActive, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(HookErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating hook {HookId}", request.Id);
                return Result.Failure<bool>(HookErrors.Unknown);
            }
        }
    }
}
