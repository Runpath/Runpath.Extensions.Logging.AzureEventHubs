using System;
using Azure.Messaging.EventHubs;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    /// <summary>
    /// Exposes the ability to process Azure Event Hub <see cref="EventData"/> instances output by
    /// the Azure Event Hubs logger provider.
    /// </summary>
    public interface IAzureEventHubsLoggerProcessor : IDisposable
    {
        void Process(EventData eventData);
    }
}
