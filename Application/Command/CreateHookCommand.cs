using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class CreateHookCommand : IRequest<Result<int>>
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Secret { get; set; }
    }

    public class CreateHookCommandHandler : IRequestHandler<CreateHookCommand, Result<int>>
    {
        private readonly IHookRepository _hookRepository;
        private readonly ILogger<CreateHookCommandHandler> _logger;

        public CreateHookCommandHandler(IHookRepository hookRepository, ILogger<CreateHookCommandHandler> logger)
        {
            _hookRepository = hookRepository;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateHookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result.Failure<int>(HookErrors.InvalidName);
                }

                if (string.IsNullOrWhiteSpace(request.Url) || !Uri.TryCreate(request.Url, UriKind.Absolute, out _))
                {
                    return Result.Failure<int>(HookErrors.InvalidUrl);
                }

                var hookId = await _hookRepository.CreateHookAsync(
                    request.ServiceId, request.Name, request.Url, request.Secret, cancellationToken);

                if (hookId == null)
                {
                    return Result.Failure<int>(HookErrors.ErrorWhileSaving);
                }

                return Result.Success(hookId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating hook for service {ServiceId}", request.ServiceId);
                return Result.Failure<int>(HookErrors.Unknown);
            }
        }
    }
}
