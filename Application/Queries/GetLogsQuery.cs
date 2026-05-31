using Application.Interfaces;
using Domain.Common;
using Domain.Common.Errors;
using Domain.Common.List;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Queries
{
    public class GetLogsQuery : IRequest<Result<PaginatedList<CustomLog>>>
    {
        public int ApiKey { get; init; }
        public int Take { get; init; }
        public int Skip { get; init; }

        public GetLogsQuery(int apiKey, int skip, int take)
        {
            ApiKey = apiKey;
            Skip = skip;
            Take = take;
        }
    }

    public class GetLogsQueryHandler : IRequestHandler<GetLogsQuery, Result<PaginatedList<CustomLog>>>
    {
        private readonly ILogger<GetLogsQueryHandler> _logger;
        private readonly ILogsRepository _logsRepository;

        public GetLogsQueryHandler(ILogger<GetLogsQueryHandler> logger, ILogsRepository logsRepository)
        {
            _logger = logger;
            _logsRepository = logsRepository;
        }

        public async Task<Result<PaginatedList<CustomLog>>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
        {
            if (request.Take <= 0 || request.Take > 100)
                return Result.Failure<PaginatedList<CustomLog>>(CustomLogsErrors.InvalidPagination);

            if (request.Skip < 0)
                return Result.Failure<PaginatedList<CustomLog>>(CustomLogsErrors.InvalidPagination);

            var logs = await _logsRepository.GetLogsAsync(request.ApiKey, request.Skip, request.Take);

            if (logs is null)
            {
                _logger.LogWarning("No logs found for ApiKey {ApiKey}", request.ApiKey);
                return Result.Failure<PaginatedList<CustomLog>>(CustomLogsErrors.NotFound);
            }

            _logger.LogInformation("Fetched {Count} logs for ApiKey {ApiKey}", logs.Count, request.ApiKey);
            return Result.Success(logs);
        }
    }
}