using System;
using Xunit;

namespace Runpath.Extensions.Logging.AzureEventHubs.Tests
{
    public class OptionsTests
    {
        [Fact]
        public void TryGetConnectionString_Should_Return_True_When_Values_Are_Present()
        {
            // Arrange
            var options = new AzureEventHubsLoggerOptions
            {
                Endpoint = new Uri("sb://test"),
                EntityPath = "test",
                SharedAccessKeyName = "test",
                SharedAccessKey = "test"
            };

            // Act
            bool result = options.TryGetConnectionString(out _);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TryGetConnectionString_Should_Return_False_When_Values_Are_Absent()
        {
            // Arrange
            var options = new AzureEventHubsLoggerOptions
            {
                Endpoint = new Uri("sb://test"),
                EntityPath = "test"
            };

            // Act
            bool result = options.TryGetConnectionString(out _);

            // Assert
            Assert.False(result);
        }
    }
}
