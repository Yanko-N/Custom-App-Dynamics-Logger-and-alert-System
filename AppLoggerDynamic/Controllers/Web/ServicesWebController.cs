using Application.Command;
using Application.Interfaces;
using Application.Queries;
using AppLoggerDynamic.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppLoggerDynamic.Controllers.Web
{
    [Route("services")]
    public class ServicesWebController : WebBaseController
    {
        private readonly IMediator _mediator;

        public ServicesWebController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var accountId = GetSessionAccountId()!.Value;
            var result = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);

            return View("~/Views/Services/Index.cshtml",
                result.IsSuccess ? result.Value.ToList() : new List<Domain.Entities.Service>());
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            return View("~/Views/Services/Create.cshtml", new ServiceFormViewModel());
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ServiceFormViewModel form, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var accountId = GetSessionAccountId()!.Value;
            var command = new CreateServiceCommand
            {
                AccountId = accountId,
                Name = form.Name,
                Environment = form.Environment,
                Version = form.Version
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error?.Message ?? "Failed to create service.");
                return View("~/Views/Services/Create.cshtml", form);
            }

            return RedirectToAction("Index");
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var result = await _mediator.Send(new GetServiceByIdQuery(id), cancellationToken);
            if (!result.IsSuccess)
                return RedirectToAction("Index");

            var svc = result.Value;
            return View("~/Views/Services/Edit.cshtml", new ServiceFormViewModel
            {
                Id = svc.Id,
                Name = svc.Name,
                Environment = svc.Environment,
                Version = svc.Version
            });
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] ServiceFormViewModel form, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var command = new UpdateServiceCommand
            {
                Id = id,
                Name = form.Name,
                Environment = form.Environment,
                Version = form.Version
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error?.Message ?? "Failed to update service.");
                form.Id = id;
                return View("~/Views/Services/Edit.cshtml", form);
            }

            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            await _mediator.Send(new DeleteServiceCommand(id), cancellationToken);
            return RedirectToAction("Index");
        }
    }
}
