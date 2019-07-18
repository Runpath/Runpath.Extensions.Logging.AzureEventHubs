using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Runpath.Extensions.Logging.AzureEventHubs.Tests
{
    internal static class TestLoggerFactoryBuilder
    {
        public static ILoggerFactory Create(Action<ILoggingBuilder> configure) =>
            new ServiceCollection()
                .AddLogging(configure)
                .BuildServiceProvider()
                .GetRequiredService<ILoggerFactory>();
    }
}
