# Runpath.Extensions.Logging.AzureEventHubs

Azure Event Hubs logger provider implementation for Microsoft.Extensions.Logging.

## Getting started

Grab the package from NuGet, which will install all dependencies.

`Install-Package Runpath.Extensions.Logging.AzureEventHubs`

## Usage

Extensive documentation for Microsoft.Extensions.Logging is available [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/) and should cover most general aspects of this library.

Once your Azure Event Hubs resource is configured in Azure, you can then add its details to your configuration. The options expose 4 values that you can later use:

```json
{
  "Logging": {
    "AzureEventHubs": {
      "FullyQualifiedNamespace": "example.servicebus.windows.net",
      "EntityPath": "my-hub",
      "SharedAccessKeyName": "my-key",
      "SharedAccessKey": "..."
    }
  }
}
```

Add this logger provider to your logging builder, supplying a delegate that creates an `EventHubProducerClient` to your specifications, for example:

```csharp
var services = new ServiceCollection();

services.AddLogging(builder => builder.AddAzureEventHubs(options =>
  options.TryGetConnectionString(out string connectionString)
    ? new EventHubProducerClient(connectionString)
    : new EventHubProducerClient(options.FullyQualifiedNamespace, options.EntityPath, new DefaultAzureCredential());
));
```

### `IAzureEventHubsLoggerFormatter`

The formatting of event data is controlled by an instance of this interface. A default implementation is supplied out of the box, which formats events as JSON:

```json
{
    "Timestamp": "2019-07-11T08:53:37.772Z",
    "LogLevel": "Information",
    "Category": "MyApplication",
    "EventId": 0,
    "Message": "Application started.",
    "Exception": null
}
```

To implement your own custom format, create your own implementation of `IAzureEventHubsLoggerFormatter` and replace the default instance in your service collection.

Custom implementations will have access to external scope data, provided by `IExternalScopeProvider`. To consume this, use the `ForEachScope` method exposed by `IAzureEventHubsLoggerFormatter`.

### `IAzureEventHubsLoggerProcessor`

The processing of event data is controlled by an instance of this interface. A default implementation is supplied that implements a queue, offloads work to a background thread, and sends event data using batches.

The options expose 2 (optional) values to customise the thresholds and queuing logic of the default processor:

```json
{
  "Logging": {
    "AzureEventHubs": {
      "QueueDepth": 1024,
      "QueueMode": "DropOldest"
    }
  }
}
```

`QueueDepth` must be a positive integer, and defaults to 1024. `QueueMode` can accept one of the values of [`BoundedChannelFullMode`](https://docs.microsoft.com/en-us/dotnet/api/system.threading.channels.boundedchannelfullmode), and defaults to `DropOldest`.

To implement your own custom processing logic, create your own implementation of `IAzureEventHubsLoggerProcessor` and replace the default instance in your service collection.
