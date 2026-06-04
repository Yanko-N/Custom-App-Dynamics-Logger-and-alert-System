using Application.Command;
using Application.Interfaces;
using System.Threading.Channels;

namespace Application.BackgroundServices
{
    /// <summary>
    /// Queue implementation using System.Threading.Channels for alert evaluations.
    /// </summary>
    public class AlertEvaluationQueue : IAlertEvaluationQueue
    {
        private readonly Channel<EvaluateAlertsCommand> _channel;

        public AlertEvaluationQueue(int capacity = 1000)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<EvaluateAlertsCommand>(options);
        }

        public ValueTask EnqueueAsync(EvaluateAlertsCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return _channel.Writer.WriteAsync(command, cancellationToken);
        }

        /// <summary>
        /// Gets the channel reader for consuming queued commands.
        /// </summary>
        public ChannelReader<EvaluateAlertsCommand> Reader => _channel.Reader;

        /// <summary>
        /// Marks the channel as complete for no more items to be written.
        /// </summary>
        public void CompleteWriter()
        {
            _channel.Writer.TryComplete();
        }
    }
}
