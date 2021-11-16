// ReSharper disable once CheckNamespace
using System.Text;

namespace Runpath.Extensions.Logging.AzureEventHubs
{
    public static class AzureEventHubsLoggerOptionsExtensions
    {
        private const char KeyValueSeparator = '=';
        private const char KeyValuePairDelimiter = ';';

        /// <summary>
        /// Attempts to build an Azure Event Hubs connections string from the component parts defined
        /// in this <see cref="AzureEventHubsLoggerOptions"/> instance.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="connectionString"></param>
        /// <returns>True, if all parts are present and valid. Otherwise, false.</returns>
        public static bool TryGetConnectionString(this AzureEventHubsLoggerOptions options, out string connectionString)
        {
            if (string.IsNullOrEmpty(options.SharedAccessKeyName) || string.IsNullOrEmpty(options.SharedAccessKey))
            {
                connectionString = null;
                return false;
            }

            try
            {
                var builder = new StringBuilder();

                if (string.IsNullOrEmpty(options.FullyQualifiedNamespace) && options.Endpoint != default)
                {
                    options.FullyQualifiedNamespace = options.Endpoint.Host;
                }

                builder.Append($"{nameof(options.FullyQualifiedNamespace)}{KeyValueSeparator}{options.FullyQualifiedNamespace}{KeyValuePairDelimiter}");
                builder.Append($"{nameof(options.EntityPath)}{KeyValueSeparator}{options.EntityPath}{KeyValuePairDelimiter}");
                builder.Append($"{nameof(options.SharedAccessKeyName)}{KeyValueSeparator}{options.SharedAccessKeyName}{KeyValuePairDelimiter}");
                builder.Append($"{nameof(options.SharedAccessKey)}{KeyValueSeparator}{options.SharedAccessKey}{KeyValuePairDelimiter}");

                connectionString = builder.ToString();
                return true;
            }
            catch
            {
                connectionString = null;
                return false;
            }
        }
    }
}
