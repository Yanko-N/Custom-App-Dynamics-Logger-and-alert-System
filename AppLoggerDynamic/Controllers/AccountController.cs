using Application.Command;
using Application.Interfaces;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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

        // GET: api/<AccountController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AccountController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateAccountRequest request)
        {
            var command = new CreateAccountCommand
            {
                Name = request.Name
            };

            Result<int> result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
               return HandleFailure(result);
            }

            return CreatedAtAction(nameof(Get), new CreateAccountResponse
            {
                Id = result.Value,
                Name = request.Name
            });

        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
