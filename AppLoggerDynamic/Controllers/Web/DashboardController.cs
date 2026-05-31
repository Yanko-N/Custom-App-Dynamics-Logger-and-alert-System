using Application.Interfaces;
using Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AppLoggerDynamic.Controllers.Web
{
    [Route("dashboard")]
    public class DashboardController : WebBaseController
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null) return auth;

            var accountId = GetSessionAccountId()!.Value;
            var result = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);

            return View(result.IsSuccess
                ? result.Value.ToList()
                : new List<Domain.Entities.Service>());
        }
    }
}
