# Webhook Relay Client 

A .NET client for the [Webhook Relay](https://webhookrelay.com) service.

It creates a websocket connection, authenticates and subscribes to the specified buckets.

Once the connection is established, it will start receiving messages and writing them to 
a channel.

You can then read messages from the channel and process them however you like. 

If the connection is lost, the client will attempt to reconnect and resubscribe.

When the client is disposed, it will close the connection and unsubscribe from the bucket(s).

It is your responsibility to handle any remaining messages in the channel.

## Installation

`dotnet add package Logicality.WebhookRelay.Client`

## Usage
```csharp

```

## Development

To run the tests you will need an account on [Webhook Relay](https://webhookrelay.com) and a bucket. Then 
you need to configure user secrets.

```bash
cd 
dotnet user-secrets set "WebhookRelayTokenKey" "<your-key>" --project test/WebhookRelay.Tests
dotnet user-secrets set "WebhookRelayTokenSecret" "<your-secret>" --project test/WebhookRelay.Tests
dotnet user-secrets set "WebhookUrl" "<your-bucket-url>" --project test/WebhookRelay.Tests
```