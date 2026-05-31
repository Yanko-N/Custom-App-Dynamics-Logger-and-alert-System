using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class DeleteAlertCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }

        public DeleteAlertCommand(int id) => Id = id;
    }

    public class DeleteAlertCommandHandler : IRequestHandler<DeleteAlertCommand, Result<bool>>
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ILogger<DeleteAlertCommandHandler> _logger;

        public DeleteAlertCommandHandler(IAlertRepository alertRepository, ILogger<DeleteAlertCommandHandler> logger)
        {
            _alertRepository = alertRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteAlertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _alertRepository.DeleteAsync(request.Id, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(AlertErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting alert {AlertId}", request.Id);
                return Result.Failure<bool>(AlertErrors.Unknown);
            }
        }
    }
}
