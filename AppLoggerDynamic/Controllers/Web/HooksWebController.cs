using Application.Command;
using Application.Interfaces;
using Application.Queries;
using AppLoggerDynamic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace AppLoggerDynamic.Controllers.Web
{
    [Route("hooks")]
    public class HooksWebController : WebBaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpClientFactory _httpClientFactory;

        public HooksWebController(IMediator mediator, IHttpClientFactory httpClientFactory)
        {
            _mediator = mediator;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] int serviceId = 0, CancellationToken cancellationToken = default)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var accountId = GetSessionAccountId()!.Value;
            var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);
            var services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new List<Domain.Entities.Service>();

            if (serviceId == 0 && services.Count > 0)
            {
                serviceId = services[0].Id;
            }

            var viewModel = new HooksViewModel { Services = services, SelectedServiceId = serviceId };

            if (serviceId > 0)
            {
                var hooksResult = await _mediator.Send(new GetHooksQuery(serviceId), cancellationToken);
                if (hooksResult.IsSuccess)
                {
                    viewModel.Hooks = hooksResult.Value.ToList();
                }
            }

            return View("~/Views/Hooks/Index.cshtml", viewModel);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create([FromQuery] int serviceId = 0, CancellationToken cancellationToken = default)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var accountId = GetSessionAccountId()!.Value;
            var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);
            var services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new List<Domain.Entities.Service>();

            return View("~/Views/Hooks/Create.cshtml", new HookFormViewModel
            {
                ServiceId = serviceId > 0 ? serviceId : (services.FirstOrDefault()?.Id ?? 0),
                Services = services
            });
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] HookFormViewModel form, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var command = new CreateHookCommand
            {
                ServiceId = form.ServiceId,
                Name = form.Name,
                Url = form.Url,
                Secret = string.IsNullOrWhiteSpace(form.Secret) ? null : form.Secret
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error?.Message ?? "Failed to create hook.");
                var accountId = GetSessionAccountId()!.Value;
                var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);
                form.Services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new();
                return View("~/Views/Hooks/Create.cshtml", form);
            }

            return RedirectToAction("Index", new { serviceId = form.ServiceId });
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var hookResult = await _mediator.Send(new GetHookByIdQuery(id), cancellationToken);
            if (!hookResult.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            var hook = hookResult.Value;
            var accountId = GetSessionAccountId()!.Value;
            var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);

            return View("~/Views/Hooks/Edit.cshtml", new HookFormViewModel
            {
                Id = hook.Id,
                ServiceId = hook.ServiceId,
                Name = hook.Name,
                Url = hook.Url,
                IsActive = hook.IsActive,
                Services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new()
            });
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] HookFormViewModel form, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var command = new UpdateHookCommand
            {
                Id = id,
                Name = form.Name,
                Url = form.Url,
                Secret = string.IsNullOrWhiteSpace(form.Secret) ? null : form.Secret,
                IsActive = form.IsActive
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error?.Message ?? "Failed to update hook.");
                var accountId = GetSessionAccountId()!.Value;
                var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);
                form.Id = id;
                form.Services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new();
                return View("~/Views/Hooks/Edit.cshtml", form);
            }

            return RedirectToAction("Index", new { serviceId = form.ServiceId });
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [FromForm] int serviceId, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            await _mediator.Send(new DeleteHookCommand(id), cancellationToken);
            return RedirectToAction("Index", new { serviceId });
        }

    }
}
