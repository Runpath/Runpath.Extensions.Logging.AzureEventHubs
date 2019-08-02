using System;
using System.Threading.Channels;
using Microsoft.Azure.EventHubs;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    public class AzureEventHubsLoggerOptions
    {
        public Uri Endpoint { get; set; }

        public string EntityPath { get; set; }

        public string SharedAccessKeyName { get; set; }

        public string SharedAccessKey { get; set; }

        public bool IncludeScopes { get; set; }

        /// <summary>
        /// The depth of the queue awaiting transmission to Azure Event Hubs.
        /// <para>Changing this value after app startup will have no effect.</para>
        /// </summary>
        public int QueueDepth { get; set; } = 1024;

        /// <summary>
        /// The mode used when queuing event data. Defaults to dropping the oldest messages in the
        /// queue to avoid blocking.
        /// <para>Changing this value after app startup will have no effect.</para>
        /// </summary>
        public BoundedChannelFullMode QueueMode { get; set; } = BoundedChannelFullMode.DropOldest;

        internal Func<AzureEventHubsLoggerOptions, EventHubClient> EventHubClientFactory { get; set; }
    }
}
