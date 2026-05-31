using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetServiceByIdQuery : IRequest<Result<Service>>
    {
        public int Id { get; init; }

        public GetServiceByIdQuery(int id) => Id = id;
    }

    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Result<Service>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetServiceByIdQueryHandler> _logger;

        public GetServiceByIdQueryHandler(IServiceRepository serviceRepository, ILogger<GetServiceByIdQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<Result<Service>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var service = await _serviceRepository.GetByIdAsync(request.Id, cancellationToken);

                if (service == null)
                {
                    _logger.LogWarning("Service {ServiceId} not found", request.Id);
                    return Result.Failure<Service>(ServiceErrors.NotFound(request.Id));
                }

                return Result.Success(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching service {ServiceId}", request.Id);
                return Result.Failure<Service>(ServiceErrors.Unknown);
            }
        }
    }
}
