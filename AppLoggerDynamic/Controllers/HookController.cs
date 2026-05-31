using Application.Command;
using Application.Queries;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static AppLoggerDynamic.Dtos.HookDtos;

namespace AppLoggerDynamic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HookController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IApiKeyRepository _apiKeyRepository;

        public HookController(IMediator mediator, IApiKeyRepository apiKeyRepository)
        {
            _mediator = mediator;
            _apiKeyRepository = apiKeyRepository;
        }

        // GET api/hook?serviceId=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int serviceId, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            Result<IEnumerable<Hook>> result = await _mediator.Send(new GetHooksQuery(serviceId), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value.Select(h => new HookResponse
            {
                Id = h.Id,
                ServiceId = h.ServiceId,
                Name = h.Name,
                Url = h.Url,
                IsActive = h.IsActive,
                CreatedAt = h.CreatedAt
            }));
        }

        // GET api/hook/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            Result<Hook> result = await _mediator.Send(new GetHookByIdQuery(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            var h = result.Value;
            return Ok(new HookResponse
            {
                Id = h.Id,
                ServiceId = h.ServiceId,
                Name = h.Name,
                Url = h.Url,
                IsActive = h.IsActive,
                CreatedAt = h.CreatedAt
            });
        }

        // POST api/hook
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateHookRequest request, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            var command = new CreateHookCommand
            {
                ServiceId = request.ServiceId,
                Name = request.Name,
                Url = request.Url,
                Secret = request.Secret
            };

            Result<int> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return CreatedAtAction(nameof(Get), new { id = result.Value }, new HookResponse
            {
                Id = result.Value,
                ServiceId = request.ServiceId,
                Name = request.Name,
                Url = request.Url,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // PUT api/hook/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateHookRequest request, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            var command = new UpdateHookCommand
            {
                Id = id,
                Name = request.Name,
                Url = request.Url,
                Secret = request.Secret,
                IsActive = request.IsActive
            };

            Result<bool> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }

        // DELETE api/hook/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            Result<bool> result = await _mediator.Send(new DeleteHookCommand(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }
    }
}
