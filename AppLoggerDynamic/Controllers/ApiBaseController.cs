using Domain.Common;
using Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AppLoggerDynamic.Controllers
{
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult HandleFailure<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Cannot handle failure for a successful result.");
            }

            if (result.Error == null)
            {
                return StatusCode(500, CreateProblemDetails("Server Error", 500, Error.Unknown("Impossible Path","Unknown Error")));
            }

            switch (result.Error.Type)
            {
                case ErrorType.Validation:
                    return BadRequest(CreateProblemDetails("Validation Error", 400, result.Error));

                case ErrorType.NotFound:
                    return NotFound(CreateProblemDetails("Not Found", 404, result.Error));

                case ErrorType.Conflict:
                    return Conflict(CreateProblemDetails("Conflict", 409, result.Error));

                case ErrorType.Unauthorized:
                    return Unauthorized(CreateProblemDetails("Unauthorized", 401, result.Error));

                case ErrorType.Failure:
                default:
                    return StatusCode(500, CreateProblemDetails("Server Error", 500, result.Error));
            }
        }

        private static ProblemDetails CreateProblemDetails(string title, int status, Error error) =>
            new()
            {
                Title = title,
                Status = status,
                Detail = error.Message,
                Extensions = { { "code", error.Code } }
            };
    }
}
