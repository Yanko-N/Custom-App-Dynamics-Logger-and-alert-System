using Application.Command;
using Application.Queries;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static AppLoggerDynamic.Dtos.ServiceDtos;

namespace AppLoggerDynamic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IApiKeyRepository _apiKeyRepository;

        public ServiceController(IMediator mediator, IApiKeyRepository apiKeyRepository)
        {
            _mediator = mediator;
            _apiKeyRepository = apiKeyRepository;
        }

        // GET api/service
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var (accountId, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            Result<IEnumerable<Service>> result = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value.Select(s => new ServiceResponse
            {
                Id = s.Id,
                AccountId = s.AccountId,
                Name = s.Name,
                Environment = s.Environment,
                Version = s.Version,
                RegisteredAt = s.RegisteredAt
            }));
        }

        // GET api/service/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            Result<Service> result = await _mediator.Send(new GetServiceByIdQuery(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            var s = result.Value;
            return Ok(new ServiceResponse
            {
                Id = s.Id,
                AccountId = s.AccountId,
                Name = s.Name,
                Environment = s.Environment,
                Version = s.Version,
                RegisteredAt = s.RegisteredAt
            });
        }

        // POST api/service
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateServiceRequest request, CancellationToken cancellationToken)
        {
            var (accountId, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            var command = new CreateServiceCommand
            {
                AccountId = accountId,
                Name = request.Name,
                Environment = request.Environment,
                Version = request.Version
            };

            Result<int> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return CreatedAtAction(nameof(Get), new { id = result.Value }, new ServiceResponse
            {
                Id = result.Value,
                AccountId = accountId,
                Name = request.Name,
                Environment = request.Environment,
                Version = request.Version,
                RegisteredAt = DateTime.UtcNow
            });
        }

        // PUT api/service/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateServiceRequest request, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            var command = new UpdateServiceCommand
            {
                Id = id,
                Name = request.Name,
                Environment = request.Environment,
                Version = request.Version
            };

            Result<bool> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }

        // DELETE api/service/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var (_, authError) = await ResolveAccountFromApiKeyHeader(_apiKeyRepository, cancellationToken);
            if (authError != null)
            {
               return authError; 
            } 

            Result<bool> result = await _mediator.Send(new DeleteServiceCommand(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }
    }
}
