using System;
using Microsoft.Azure.EventHubs;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    public static class AzureEventHubsLoggerOptionsExtensions
    {
        /// <summary>
        /// Attempts to build an Azure Event Hubs connections string from the component parts defined
        /// in this <see cref="AzureEventHubsLoggerOptions"/> instance.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="connectionString"></param>
        /// <returns>True, if all parts are present and valid. Otherwise, false.</returns>
        public static bool TryGetConnectionString(this AzureEventHubsLoggerOptions options, out string connectionString)
        {
            try
            {
                var builder = new EventHubsConnectionStringBuilder(options.Endpoint, options.EntityPath, options.SharedAccessKeyName, options.SharedAccessKey);

                connectionString = builder.ToString();
                return true;
            }
            catch (ArgumentException)
            {
                connectionString = null;
                return false;
            }
        }
    }
}
