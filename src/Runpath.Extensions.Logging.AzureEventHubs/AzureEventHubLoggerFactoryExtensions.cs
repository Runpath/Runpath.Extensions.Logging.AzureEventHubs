using System;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Runpath.Extensions.Logging.AzureEventHubs;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging
{
    public static class AzureEventHubLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds a AzureEventHubs logger named 'AzureEventHubs' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="eventHubClientFactory"></param>
        public static ILoggingBuilder AddAzureEventHubs(this ILoggingBuilder builder,
            Func<AzureEventHubsLoggerOptions, EventHubProducerClient> eventHubClientFactory)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddConfiguration();

            builder.Services.TryAddSingleton<IAzureEventHubsLoggerFormatter, DefaultAzureEventHubsLoggerFormatter>();
            builder.Services.TryAddSingleton<IAzureEventHubsLoggerProcessor, DefaultAzureEventHubsLoggerProcessor>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, AzureEventHubsLoggerProvider>());
            builder.Services.RegisterProviderOptions<AzureEventHubsLoggerOptions, AzureEventHubsLoggerProvider>();
            builder.Services.Configure<AzureEventHubsLoggerOptions>(opts => opts.EventHubClientFactory = eventHubClientFactory);

            return builder;
        }

        /// <summary>
        /// Adds a AzureEventHubs logger named 'AzureEventHubs' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="eventHubClientFactory"></param>
        /// <param name="configure"></param>
        public static ILoggingBuilder AddAzureEventHubs(this ILoggingBuilder builder,
            Func<AzureEventHubsLoggerOptions, EventHubProducerClient> eventHubClientFactory,
            Action<AzureEventHubsLoggerOptions> configure)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddAzureEventHubs(eventHubClientFactory);
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
