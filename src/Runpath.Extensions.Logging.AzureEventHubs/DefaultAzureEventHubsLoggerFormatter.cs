using System;
using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    /// <summary>
    /// Default implementation for <see cref="IAzureEventHubsLoggerFormatter"/>.
    /// </summary>
    internal class DefaultAzureEventHubsLoggerFormatter : IAzureEventHubsLoggerFormatter
    {
        private static readonly Lazy<JsonSerializerSettings> JsonSerializerSettings = new Lazy<JsonSerializerSettings>(() =>
            new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Error = (_, args) => args.ErrorContext.Handled = true,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = { new StringEnumConverter() }
            });

        public ScopeCallback ForEachScope { get; set; }

        public EventData Format<TState>(string category, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var model = new
            {
                Timestamp = DateTime.UtcNow,
                Category = category,
                LogLevel = logLevel,
                EventId = eventId.Id,
                Message = formatter(state, exception),
                Exception = exception
            };

            string json = JsonConvert.SerializeObject(model, JsonSerializerSettings.Value);

            return new EventData(Encoding.UTF8.GetBytes(json));
        }
    }
}
