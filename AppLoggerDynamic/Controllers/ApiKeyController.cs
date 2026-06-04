using Application.Command;
using Application.Queries;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using static AppLoggerDynamic.Dtos.ApiKeyDtos;

namespace AppLoggerDynamic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiKeyController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public ApiKeyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/apikey?accountId=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int accountId, CancellationToken cancellationToken)
        {
            Result<IEnumerable<ApiKey>> result = await _mediator.Send(new GetApiKeysQuery(accountId), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value.Select(key => new ApiKeyResponse
            {
                Id = key.Id,
                AccountId = key.AccountId,
                Label = key.Label,
                IsActive = key.IsActive,
                CreatedAt = key.CreatedAt,
                ExpiresAt = key.ExpiresAt,
                LastUsedAt = key.LastUsedAt,
                KeyHashPreview = key.KeyHash[..8] + "..."
            }));
        }

        // GET api/apikey/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            Result<ApiKey> result = await _mediator.Send(new GetApiKeyByIdQuery(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            var key = result.Value;
            return Ok(new ApiKeyResponse
            {
                Id = key.Id,
                AccountId = key.AccountId,
                Label = key.Label,
                IsActive = key.IsActive,
                CreatedAt = key.CreatedAt,
                ExpiresAt = key.ExpiresAt,
                LastUsedAt = key.LastUsedAt,
                KeyHashPreview = key.KeyHash[..8] + "..."
            });
        }

        // POST api/apikey
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateApiKeyRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateApiKeyCommand
            {
                AccountId = request.AccountId,
                Label = request.Label,
                ExpiresAt = request.ExpiresAt
            };

            Result<ApiKeyCreatedResult> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, new CreateApiKeyResponse
            {
                Id = result.Value.Id,
                Label = result.Value.Label,
                RawKey = result.Value.RawKey
            });
        }

        // PUT api/apikey/5/revoke
        [HttpPut("{id}/revoke")]
        public async Task<IActionResult> Revoke(int id, CancellationToken cancellationToken)
        {
            Result<bool> result = await _mediator.Send(new RevokeApiKeyCommand(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }

        // DELETE api/apikey/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            Result<bool> result = await _mediator.Send(new DeleteApiKeyCommand(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }
    }
}
