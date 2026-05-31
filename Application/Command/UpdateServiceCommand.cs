using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class UpdateServiceCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }

    public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Result<bool>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<UpdateServiceCommandHandler> _logger;

        public UpdateServiceCommandHandler(IServiceRepository serviceRepository, ILogger<UpdateServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result.Failure<bool>(ServiceErrors.InvalidName);
                }

                var success = await _serviceRepository.UpdateAsync(
                    request.Id, request.Name, request.Environment, request.Version, cancellationToken);

                if (!success)
                {
                    return Result.Failure<bool>(ServiceErrors.NotFound(request.Id));
                }

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating service {ServiceId}", request.Id);
                return Result.Failure<bool>(ServiceErrors.Unknown);
            }
        }
    }
}
