using System;
using Azure.Messaging.EventHubs;
using Microsoft.Extensions.Logging;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    /// <summary>
    /// Exposes the ability to control the format of logger messages output by the Azure Event Hubs
    /// logger provider.
    /// </summary>
    public interface IAzureEventHubsLoggerFormatter
    {
        /// <summary>
        /// Executes a callback for each currently active scope object, in order of creation.
        /// </summary>
        ScopeCallback ForEachScope { get; set; }

        /// <summary>
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="category"></param>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        EventData Format<TState>(string category, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
    }

    /// <summary>
    /// Executes a callback for each currently active scope object, in order of creation.
    /// </summary>
    /// <param name="callback">The callback to be executed for every scope object</param>
    /// <param name="state">The state object to be passed into the callback</param>
    public delegate void ScopeCallback(Action<object, object> callback, object state);
}
