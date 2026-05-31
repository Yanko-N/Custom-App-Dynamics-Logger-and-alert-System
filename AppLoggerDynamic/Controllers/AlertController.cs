using Application.Command;
using Application.Queries;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static AppLoggerDynamic.Dtos.AlertDtos;

namespace AppLoggerDynamic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IApiKeyRepository _apiKeyRepository;

        public AlertController(IMediator mediator, IApiKeyRepository apiKeyRepository)
        {
            _mediator = mediator;
            _apiKeyRepository = apiKeyRepository;
        }

        // GET api/alert?serviceId=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int serviceId, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
                return authError; 
            }

            Result<IEnumerable<Alert>> result = await _mediator.Send(new GetAlertsQuery(serviceId), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value.Select(alert => new AlertResponse
            {
                Id = alert.Id,
                ServiceId = alert.ServiceId,
                Name = alert.Name,
                Level = alert.Level,
                Condition = alert.Condition,
                ThresholdValue = alert.ThresholdValue,
                WindowSeconds = alert.WindowSeconds,
                IsActive = alert.IsActive
            }));
        }

        // GET api/alert/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            Result<Alert> result = await _mediator.Send(new GetAlertByIdQuery(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            var alert = result.Value;
            return Ok(new AlertResponse
            {
                Id = alert.Id,
                ServiceId = alert.ServiceId,
                Name = alert.Name,
                Level = alert.Level,
                Condition = alert.Condition,
                ThresholdValue = alert.ThresholdValue,
                WindowSeconds = alert.WindowSeconds,
                IsActive = alert.IsActive
            });
        }

        // POST api/alert
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateAlertRequest request, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
                return authError; 
            }

            var command = new CreateAlertCommand
            {
                ServiceId = request.ServiceId,
                Name = request.Name,
                Level = request.Level,
                Condition = request.Condition,
                ThresholdValue = request.ThresholdValue,
                WindowSeconds = request.WindowSeconds
            };

            Result<int> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return CreatedAtAction(nameof(Get), new { id = result.Value }, new AlertResponse
            {
                Id = result.Value,
                ServiceId = request.ServiceId,
                Name = request.Name,
                Level = request.Level.ToUpperInvariant(),
                Condition = request.Condition,
                ThresholdValue = request.ThresholdValue,
                WindowSeconds = request.WindowSeconds,
                IsActive = true
            });
        }

        // PUT api/alert/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateAlertRequest request, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            var command = new UpdateAlertCommand
            {
                Id = id,
                Name = request.Name,
                Level = request.Level,
                Condition = request.Condition,
                ThresholdValue = request.ThresholdValue,
                WindowSeconds = request.WindowSeconds,
                IsActive = request.IsActive
            };

            Result<bool> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }

        // DELETE api/alert/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
                return authError; 
            }

            Result<bool> result = await _mediator.Send(new DeleteAlertCommand(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }
    }
}
