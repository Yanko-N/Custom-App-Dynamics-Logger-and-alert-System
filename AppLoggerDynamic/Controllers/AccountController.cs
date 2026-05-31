using Application.Command;
using Application.Queries;
using Application.Interfaces;
using Domain.Common;
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

        // GET: api/Account
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            // Using a Query for fetching data
            var query = new GetAccountsQuery();
            Result<IEnumerable<AccountResponse>> result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value);
        }

        // GET api/Account/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var query = new GetAccountByIdQuery(id);
            Result<AccountResponse> result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value);
        }

        // POST api/Account
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

        // PUT api/Account/5
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

        // DELETE api/Account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteAccountCommand(id);
            Result<bool> result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return NoContent(); // 204 No Content for successful deletion
        }
    }
}