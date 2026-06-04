using Application.Command;
using Application.Queries;
using Application.Interfaces;
using Domain.Common;
using Domain.Common.List;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static AppLoggerDynamic.Dtos.LogDtos;

namespace AppLoggerDynamic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IApiKeyRepository _apiKeyRepository;

        public LogController(IMediator mediator, IApiKeyRepository apiKeyRepository)
        {
            _mediator = mediator;
            _apiKeyRepository = apiKeyRepository;
        }

        // GET api/log?serviceId=1&skip=0&take=50
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] int serviceId,[FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken cancellationToken = default)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            var query = new GetLogsQuery(serviceId, skip, take);
            Result<PaginatedList<CustomLog>> result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(new
            {
                Data = result.Value.Select(l => new LogResponse
                {
                    Id = l.Id,
                    ServiceId = l.ServiceId,
                    Timestamp = l.Timestamp,
                    Level = l.Level,
                    TraceId = l.TraceId,
                    Message = l.Message,
                    StackTrace = l.StackTrace
                }),
                result.Value.TotalCount,
                result.Value.TotalPages,
                result.Value.CurrentPage,
                result.Value.HasNextPage,
                result.Value.HasPreviousPage
            });
        }

        // POST api/log
        [HttpPost]
        public async Task<IActionResult> Ingest([FromBody] IngestLogRequest request, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            var command = new IngestLogCommand
            {
                ServiceId = request.ServiceId,
                Level = request.Level,
                Message = request.Message,
                StackTrace = request.StackTrace,
                TraceId = request.TraceId
            };

            Result<long> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return StatusCode(201, new { Id = result.Value });
        }
    }
}
