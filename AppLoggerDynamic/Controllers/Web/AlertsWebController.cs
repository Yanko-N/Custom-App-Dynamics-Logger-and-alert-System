using Application.Command;
using Application.Interfaces;
using Application.Queries;
using AppLoggerDynamic.ViewModels;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppLoggerDynamic.Controllers.Web
{
    [Route("alerts")]
    public class AlertsWebController : WebBaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogsRepository _logsRepository;

        public AlertsWebController(IMediator mediator, ILogsRepository logsRepository)
        {
            _mediator = mediator;
            _logsRepository = logsRepository;
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

            var viewModel = new AlertsViewModel 
            {
                Services = services,
                SelectedServiceId = serviceId 
            };

            if (serviceId > 0)
            {
                var alertsResult = await _mediator.Send(new GetAlertsQuery(serviceId), cancellationToken);
                if (alertsResult.IsSuccess)
                {
                    foreach (var alert in alertsResult.Value)
                    {
                        var windowStart = DateTime.UtcNow.AddSeconds(-alert.WindowSeconds);
                        var count = await _logsRepository.CountLogsInWindowAsync(
                            alert.ServiceId, alert.Level, windowStart, alert.MessagePattern, cancellationToken);

                        bool violating;
                        switch (alert.Condition)
                        {
                            case "GreaterThan":
                                violating = count > alert.ThresholdValue;
                                break;
                            case "LessThan":
                                violating = count < alert.ThresholdValue;
                                break;
                            case "Equals":
                                violating = count == alert.ThresholdValue;
                                break;
                            default:
                                violating = false;
                                break;
                        }

                        viewModel.AlertStatuses.Add(new AlertStatusItem
                        {
                            Alert = alert,
                            CurrentCount = count,
                            IsViolating = violating
                        });
                    }
                }
            }

            return View("~/Views/Alerts/Index.cshtml", viewModel);
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

            return View("~/Views/Alerts/Create.cshtml", new AlertFormViewModel
            {
                ServiceId = serviceId > 0 ? serviceId : (services.FirstOrDefault()?.Id ?? 0),
                Services = services
            });
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AlertFormViewModel form, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var command = new CreateAlertCommand
            {
                ServiceId = form.ServiceId,
                Name = form.Name,
                Level = form.Level,
                Condition = form.Condition,
                ThresholdValue = form.ThresholdValue,
                WindowSeconds = form.WindowSeconds,
                MessagePattern = form.MessagePattern
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error?.Message ?? "Failed to create alert.");
                var accountId = GetSessionAccountId()!.Value;
                var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);
                form.Services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new();
                return View("~/Views/Alerts/Create.cshtml", form);
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

            var alertResult = await _mediator.Send(new GetAlertByIdQuery(id), cancellationToken);
            if (!alertResult.IsSuccess)
                return RedirectToAction("Index");

            var alert = alertResult.Value;
            var accountId = GetSessionAccountId()!.Value;
            var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);

            return View("~/Views/Alerts/Edit.cshtml", new AlertFormViewModel
            {
                Id = alert.Id,
                ServiceId = alert.ServiceId,
                Name = alert.Name,
                Level = alert.Level,
                Condition = alert.Condition,
                ThresholdValue = alert.ThresholdValue,
                WindowSeconds = alert.WindowSeconds,
                IsActive = alert.IsActive,
                MessagePattern = alert.MessagePattern,
                Services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new()
            });
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] AlertFormViewModel form, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null)
            {
                return auth;
            }

            var command = new UpdateAlertCommand
            {
                Id = id,
                Name = form.Name,
                Level = form.Level,
                Condition = form.Condition,
                ThresholdValue = form.ThresholdValue,
                WindowSeconds = form.WindowSeconds,
                IsActive = form.IsActive,
                MessagePattern = form.MessagePattern
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error?.Message ?? "Failed to update alert.");
                var accountId = GetSessionAccountId()!.Value;
                var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);
                form.Services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new();
                return View("~/Views/Alerts/Edit.cshtml", form);
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

            await _mediator.Send(new DeleteAlertCommand(id), cancellationToken);
            return RedirectToAction("Index", new { serviceId });
        }
    }
}
