using System;
using Microsoft.Extensions.Logging;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    internal class AzureEventHubsLogger : ILogger
    {
        private readonly string name;
        private readonly IAzureEventHubsLoggerFormatter loggerFormatter;
        private readonly IAzureEventHubsLoggerProcessor loggerProcessor;

        internal AzureEventHubsLogger(string name, IAzureEventHubsLoggerFormatter loggerFormatter, IAzureEventHubsLoggerProcessor loggerProcessor)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.loggerFormatter = loggerFormatter ?? throw new ArgumentNullException(nameof(loggerFormatter));
            this.loggerProcessor = loggerProcessor ?? throw new ArgumentNullException(nameof(loggerProcessor));
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        internal AzureEventHubsLoggerOptions Options { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var eventData = this.loggerFormatter.Format(this.name, logLevel, eventId, state, exception, formatter);

            this.loggerProcessor.Process(eventData);
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}
