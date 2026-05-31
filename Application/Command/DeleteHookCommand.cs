using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class DeleteHookCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }

        public DeleteHookCommand(int id) => Id = id;
    }

    public class DeleteHookCommandHandler : IRequestHandler<DeleteHookCommand, Result<bool>>
    {
        private readonly IHookRepository _hookRepository;
        private readonly ILogger<DeleteHookCommandHandler> _logger;

        public DeleteHookCommandHandler(IHookRepository hookRepository, ILogger<DeleteHookCommandHandler> logger)
        {
            _hookRepository = hookRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteHookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _hookRepository.DeleteAsync(request.Id, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(HookErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting hook {HookId}", request.Id);
                return Result.Failure<bool>(HookErrors.Unknown);
            }
        }
    }
}
