using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class CreateServiceCommand : IRequest<Result<int>>
    {
        public int AccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }

    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Result<int>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<CreateServiceCommandHandler> _logger;

        public CreateServiceCommandHandler(IServiceRepository serviceRepository, ILogger<CreateServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Result.Failure<int>(ServiceErrors.InvalidName);
                }

                var serviceId = await _serviceRepository.CreateServiceAsync(
                    request.AccountId, request.Name, request.Environment, request.Version, cancellationToken);

                if (serviceId == null)
                {
                    return Result.Failure<int>(ServiceErrors.ErrorWhileSaving);
                }

                return Result.Success(serviceId.Value);
            }
            catch (Domain.Common.Exceptions.ServiceNameConflictException)
            {
                return Result.Failure<int>(ServiceErrors.NameTaken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating service for account {AccountId}", request.AccountId);
                return Result.Failure<int>(ServiceErrors.Unknown);
            }
        }
    }
}
