using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.BackgroundServices
{
    /// <summary>
    /// Background service that processes alert evaluations from the queue.
    /// </summary>
    public class AlertEvaluationBackgroundService : BackgroundService
    {
        private readonly AlertEvaluationQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AlertEvaluationBackgroundService> _logger;

        public AlertEvaluationBackgroundService(
            IAlertEvaluationQueue queue,
            IServiceProvider serviceProvider,
            ILogger<AlertEvaluationBackgroundService> logger)
        {
            _queue = queue as AlertEvaluationQueue ?? throw new InvalidOperationException("Queue must be of type AlertEvaluationQueue");
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Alert Evaluation Background Service started");

            try
            {
                await foreach (var command in _queue.Reader.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateAsyncScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            await mediator.Send(command, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing alert evaluation for service {ServiceId}", command.ServiceId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Alert Evaluation Background Service is stopping");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Alert Evaluation Background Service");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Alert Evaluation Background Service stopping");
            _queue.CompleteWriter();
            await base.StopAsync(cancellationToken);
        }
    }
}
