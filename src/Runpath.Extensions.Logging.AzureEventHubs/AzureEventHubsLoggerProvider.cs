using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    [ProviderAlias("AzureEventHubs")]
    public class AzureEventHubsLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, AzureEventHubsLogger> loggers;
        private readonly IAzureEventHubsLoggerFormatter formatter;
        private readonly IAzureEventHubsLoggerProcessor processor;

        private IDisposable optionsReloadToken;
        private IExternalScopeProvider scopeProvider;

        public AzureEventHubsLoggerProvider(IAzureEventHubsLoggerFormatter formatter, IAzureEventHubsLoggerProcessor processor)
        {
            this.formatter = formatter;
            this.processor = processor;
            this.loggers = new ConcurrentDictionary<string, AzureEventHubsLogger>();

            SetScopeProvider(NullExternalScopeProvider.Instance);
        }

        /// <inheritdoc/>
        public ILogger CreateLogger(string name) => this.loggers.GetOrAdd(name,
            _ => new AzureEventHubsLogger(name, this.formatter, this.processor)
            {
                ScopeProvider = this.scopeProvider
            });

        /// <inheritdoc/>
        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;

            this.formatter.ForEachScope = scopeProvider.ForEachScope;

            foreach (var logger in this.loggers)
            {
                logger.Value.ScopeProvider = this.scopeProvider;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.optionsReloadToken?.Dispose();
            this.processor.Dispose();
        }
    }
}
