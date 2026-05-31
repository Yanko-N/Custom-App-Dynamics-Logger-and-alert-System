using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class DeleteServiceCommand : IRequest<Result<bool>>
    {
        public int Id { get; init; }

        public DeleteServiceCommand(int id) => Id = id;
    }

    public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Result<bool>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<DeleteServiceCommandHandler> _logger;

        public DeleteServiceCommandHandler(IServiceRepository serviceRepository, ILogger<DeleteServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _serviceRepository.DeleteAsync(request.Id, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(ServiceErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting service {ServiceId}", request.Id);
                return Result.Failure<bool>(ServiceErrors.Unknown);
            }
        }
    }
}
