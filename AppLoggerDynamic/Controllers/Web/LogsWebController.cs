using Application.Interfaces;
using Application.Queries;
using AppLoggerDynamic.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppLoggerDynamic.Controllers.Web
{
    [Route("logs")]
    public class LogsWebController : WebBaseController
    {
        private readonly IMediator _mediator;

        public LogsWebController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(
            [FromQuery] int serviceId = 0,
            [FromQuery] string level = "",
            [FromQuery] int page = 1,
            [FromQuery] int take = 50,
            CancellationToken cancellationToken = default)
        {
            var auth = RequireAuth();
            if (auth != null) return auth;

            if (page < 1) page = 1;

            var accountId = GetSessionAccountId()!.Value;
            var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);
            var services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new List<Domain.Entities.Service>();

            if (serviceId == 0 && services.Count > 0)
                serviceId = services[0].Id;

            var vm = new LogsViewModel
            {
                Services = services,
                SelectedServiceId = serviceId,
                Level = level,
                Skip = page,   // Skip maps to currentPage in PaginatedList
                Take = take
            };

            if (serviceId > 0)
            {
                // GetLogsQuery.Skip is used as currentPage by the repository
                var logsResult = await _mediator.Send(new GetLogsQuery(serviceId, page, take), cancellationToken);
                if (logsResult.IsSuccess)
                    vm.Logs = logsResult.Value;
            }

            return View("~/Views/Logs/Index.cshtml", vm);
        }
    }
}
