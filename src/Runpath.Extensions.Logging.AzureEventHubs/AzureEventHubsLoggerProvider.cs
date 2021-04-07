using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    [ProviderAlias("AzureEventHubs")]
    public class AzureEventHubsLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly IOptionsMonitor<AzureEventHubsLoggerOptions> options;
        private readonly ConcurrentDictionary<string, AzureEventHubsLogger> loggers;
        private readonly IAzureEventHubsLoggerFormatter formatter;
        private readonly IAzureEventHubsLoggerProcessor processor;

        private IDisposable optionsReloadToken;
        private IExternalScopeProvider scopeProvider;

        public AzureEventHubsLoggerProvider(IOptionsMonitor<AzureEventHubsLoggerOptions> options, IAzureEventHubsLoggerFormatter formatter, IAzureEventHubsLoggerProcessor processor)
        {
            this.options = options;
            this.formatter = formatter;
            this.processor = processor;
            this.loggers = new ConcurrentDictionary<string, AzureEventHubsLogger>();

            ReloadLoggerOptions(options.CurrentValue);
            this.optionsReloadToken = this.options.OnChange(ReloadLoggerOptions);

            SetScopeProvider(NullExternalScopeProvider.Instance);
        }

        private void ReloadLoggerOptions(AzureEventHubsLoggerOptions options)
        {
            foreach (var logger in this.loggers)
            {
                logger.Value.Options = options;
            }
        }

        /// <inheritdoc/>
        public ILogger CreateLogger(string name) => this.loggers.GetOrAdd(name,
            _ => new AzureEventHubsLogger(name, this.formatter, this.processor)
            {
                Options = this.options.CurrentValue,
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
