using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Options;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    /// <summary>
    /// Default implementation for <see cref="IAzureEventHubsLoggerProcessor"/>.
    /// </summary>
    internal class DefaultAzureEventHubsLoggerProcessor : IAzureEventHubsLoggerProcessor
    {
        private readonly IOptionsMonitor<AzureEventHubsLoggerOptions> options;
        private readonly Channel<EventData> channel;

        private IDisposable optionsReloadToken;
        private EventHubClient eventHubClient;

        public DefaultAzureEventHubsLoggerProcessor(IOptionsMonitor<AzureEventHubsLoggerOptions> options)
        {
            this.options = options;

            ReloadLoggerOptions(options.CurrentValue);

            this.channel = Channel.CreateBounded<EventData>(new BoundedChannelOptions(options.CurrentValue.QueueDepth)
            {
                FullMode = options.CurrentValue.QueueMode
            });

            Task.Factory.StartNew(ReadChannelAsync,
                CancellationToken.None,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }

        private void ReloadLoggerOptions(AzureEventHubsLoggerOptions options)
        {
            this.eventHubClient = options.EventHubClientFactory?.Invoke(options);

            this.optionsReloadToken = this.options.OnChange(ReloadLoggerOptions);
        }

        public void Process(EventData eventData) => this.channel.Writer.TryWrite(eventData);

        private async Task ReadChannelAsync()
        {
            while (await this.channel.Reader.WaitToReadAsync())
            {
                // No client is available, so briefly pause before retrying.
                if (this.eventHubClient is null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

                    continue;
                }

                var eventDataBatch = this.eventHubClient.CreateBatch();

                while (this.channel.Reader.TryRead(out var eventData))
                {
                    // Attempt to add the current event data to existing batch.
                    if (eventDataBatch.TryAdd(eventData))
                    {
                        // There was space available, try to read more event data.
                        continue;
                    }

                    // There was not enough space available, so send the current batch and create a
                    // new one.
                    await TrySendAsync(eventDataBatch).ConfigureAwait(false);
                    eventDataBatch = this.eventHubClient.CreateBatch();

                    // Attempt to add the current event data to new batch.
                    eventDataBatch.TryAdd(eventData);
                }

                // No more event data is currently available, so send the current batch.
                await TrySendAsync(eventDataBatch).ConfigureAwait(false);
            }
        }

        private async Task TrySendAsync(EventDataBatch eventDataBatch)
        {
            try
            {
                await this.eventHubClient.SendAsync(eventDataBatch).ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
        }

        public void Dispose()
        {
            this.channel.Writer.Complete();

            this.optionsReloadToken?.Dispose();
        }
    }
}
