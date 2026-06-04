using Application.Command;
using Application.Interfaces;
using Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AppLoggerDynamic.Controllers.Web
{
    [Route("debug")]
    public class DebugController : WebBaseController
    {
        private readonly IMediator _mediator;
        private static readonly Random _rng = new();

        public DebugController(IMediator mediator)
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
            var servicesResult = await _mediator.Send(new GetServicesQuery(accountId), cancellationToken);
            var services = servicesResult.IsSuccess ? servicesResult.Value.ToList() : new List<Domain.Entities.Service>();

            return View("~/Views/Debug/Index.cshtml", services);
        }

        [HttpPost("ingest")]
        public async Task<IActionResult> Ingest([FromBody] IngestRequest req, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null) return Json(new { ok = false, error = "Unauthenticated" });

            var command = new IngestLogCommand
            {
                ServiceId = req.ServiceId,
                Level = req.Level,
                Message = req.Message,
                StackTrace = string.IsNullOrWhiteSpace(req.StackTrace) ? null : req.StackTrace,
                TraceId = Guid.NewGuid()
            };

            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                return Json(new { ok = false, error = result.Error?.Message });
            }

            return Json(new { ok = true, id = result.Value });
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> Bulk([FromBody] BulkRequest req, CancellationToken cancellationToken)
        {
            var auth = RequireAuth();
            if (auth != null) return Json(new { ok = false, error = "Unauthenticated" });

            var count = Math.Clamp(req.Count, 1, 200);
            var results = new List<object>();

            var templates = ScenarioTemplates(req.Scenario);

            for (int i = 0; i < count; i++)
            {
                var tpl = templates[_rng.Next(templates.Count)];
                var command = new IngestLogCommand
                {
                    ServiceId = req.ServiceId,
                    Level = tpl.Level,
                    Message = tpl.Message,
                    StackTrace = tpl.StackTrace,
                    TraceId = Guid.NewGuid()
                };
                var result = await _mediator.Send(command, cancellationToken);
                results.Add(new { ok = result.IsSuccess, level = tpl.Level, message = tpl.Message });
            }

            var succeeded = results.Count(x => (bool)x.GetType().GetProperty("ok")!.GetValue(x)!);
            return Json(new { ok = true, total = count, succeeded });
        }

        private static List<(string Level, string Message, string? StackTrace)> ScenarioTemplates(string scenario)
        {
            switch (scenario)
            {
                case "errors":
                    return new List<(string Level, string Message, string? StackTrace)>
                    {
                        ("ERROR", "NullReferenceException: Object reference not set to an instance of an object.", "   at MyApp.Services.OrderService.ProcessOrder(Order order) in OrderService.cs:line 42\n   at MyApp.Controllers.OrderController.Post(OrderRequest req) in OrderController.cs:line 18"),
                        ("ERROR", "Database connection timeout after 30s - retrying (attempt 3/3).", null),
                        ("ERROR", "Unhandled exception in background worker: InvalidOperationException.", "   at MyApp.Workers.EmailWorker.SendAsync(Guid messageId) in EmailWorker.cs:line 87"),
                        ("ERROR", "HTTP 500 returned from downstream payment API.", null),
                        ("FATAL", "Out of memory exception - process terminating.", "   at System.GC.Collect()\n   at MyApp.Program.Main(String[] args) in Program.cs:line 5"),
                    };
                case "normal":
                    return new List<(string Level, string Message, string? StackTrace)>
                    {
                        ("INFO",  "Request GET /api/products completed in 42ms.", null),
                        ("INFO",  "User authenticated successfully.", null),
                        ("DEBUG", "Cache hit for key 'products:featured'.", null),
                        ("INFO",  "Background job 'CleanupExpiredSessions' completed. Removed 17 records.", null),
                        ("DEBUG", "Email queued for delivery to user@example.com.", null),
                        ("INFO",  "Health check passed - all dependencies reachable.", null),
                        ("WARN",  "Response time exceeded 500ms threshold (took 623ms).", null),
                        ("DEBUG", "Deserialized 48 items from cache.", null),
                    };
                case "incident":
                    return new List<(string Level, string Message, string? StackTrace)>
                    {
                        ("WARN",  "Latency spike detected - p99 above 2s.", null),
                        ("ERROR", "Circuit breaker opened for service 'InventoryService'.", null),
                        ("ERROR", "3 retries exhausted for order #84921 - sending to DLQ.", "   at MyApp.Messaging.RetryPolicy.ExecuteAsync() in RetryPolicy.cs:line 61"),
                        ("FATAL", "Critical: database primary replica unreachable.", "   at Npgsql.NpgsqlConnection.Open()\n   at MyApp.Data.AppDbContext.SaveChangesAsync()"),
                        ("ERROR", "Payment gateway timeout - transaction rolled back.", null),
                        ("ERROR", "Dependency health check FAILED: Redis cluster not responding.", null),
                    };
                default:
                    return new List<(string Level, string Message, string? StackTrace)>
                    {
                        ("INFO",  "Service started.", null),
                        ("DEBUG", "Configuration loaded.", null),
                    };
            }
        }

        public record IngestRequest(int ServiceId, string Level, string Message, string? StackTrace);
        public record BulkRequest(int ServiceId, int Count, string Scenario);
    }
}
