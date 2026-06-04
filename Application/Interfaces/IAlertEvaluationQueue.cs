using Application.Command;

namespace Application.Interfaces
{
    /// <summary>
    /// Interface for enqueueing alert evaluation commands.
    /// </summary>
    public interface IAlertEvaluationQueue
    {
        /// <summary>
        /// Enqueues an alert evaluation command to be processed by the background service.
        /// </summary>
        ValueTask EnqueueAsync(EvaluateAlertsCommand command, CancellationToken cancellationToken = default);
    }
}
