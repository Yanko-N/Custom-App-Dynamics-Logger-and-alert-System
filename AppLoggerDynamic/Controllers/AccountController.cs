using Application.Command;
using Application.Queries;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using static AppLoggerDynamic.Dtos.AccountDtos;

namespace AppLoggerDynamic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/account
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            Result<IEnumerable<Account>> result = await _mediator.Send(new GetAccountsQuery(), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value.Select(a => new AccountResponse
            {
                Id = a.Id,
                Name = a.Name,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            }));
        }

        // GET api/account/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            Result<Account> result = await _mediator.Send(new GetAccountByIdQuery(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            var a = result.Value;
            return Ok(new AccountResponse
            {
                Id = a.Id,
                Name = a.Name,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            });
        }

        // POST api/account
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateAccountCommand { Name = request.Name };
            Result<int> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return CreatedAtAction(nameof(Get), new { id = result.Value }, new CreateAccountResponse
            {
                Id = result.Value,
                Name = request.Name
            });
        }

        // PUT api/account/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateAccountRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateAccountCommand
            {
                Id = id,
                Name = request.Name,
                IsActive = request.IsActive
            };

            Result<bool> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }

        // DELETE api/account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            Result<bool> result = await _mediator.Send(new DeleteAccountCommand(id), cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent();
        }
    }
}
