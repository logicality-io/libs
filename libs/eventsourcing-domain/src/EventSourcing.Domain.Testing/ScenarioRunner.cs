using System.Text.Json;
using KellermanSoftware.CompareNetObjects;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace Logicality.EventSourcing.Domain.Testing;

public class ScenarioRunner(
    IStreamStore                                  store,
    MessageNameResolver                           messageNameResolver,
    MessageTypeResolver                           messageTypeResolver,
    JsonSerializerOptions                         settings,
    Func<object, CancellationToken, Task<object>> dispatcher)
{
    public async Task<object> RunAsync(Scenario scenario, CancellationToken cancellationToken = default)
    {
        var position = await WriteGivens(scenario.Givens, cancellationToken);
            
        try
        {
            var result = await dispatcher(scenario.When, cancellationToken);
            var thens  = await ReadThens(position, cancellationToken);
            var config = new ComparisonConfig
            {
                MaxDifferences = int.MaxValue,
                MaxStructDepth = 5,
            };
            var comparer        = new CompareLogic(config);
            var eventComparison = comparer.Compare(scenario.Thens, thens);
            if (!eventComparison.AreEqual)
            {
                return scenario.ButRecordedOtherEvents(thens);
            }
            var resultComparison = comparer.Compare(scenario.Result, result);
            if (!resultComparison.AreEqual)
            {
                return scenario.ButReturnedOtherResult(result);
            }
            return scenario.Pass(result);

        }
        catch (Exception exception)
        {
            return scenario.ButThrewException(exception);
        }
    }

    private async Task<long> WriteGivens(IReadOnlyCollection<RecordedEvent> givens, CancellationToken cancellationToken)
    {
        var position = Position.Start;
        foreach (var stream in givens.GroupBy(@event => @event.Stream))
        {
            var appendResult = await store.AppendToStream(
                new StreamId(stream.Key.ToString()),
                ExpectedVersion.NoStream,
                stream
                    .Select(@event => new NewStreamMessage(
                        Guid.NewGuid(),
                        messageNameResolver(@event.Message.GetType()),
                        JsonSerializer.Serialize(@event.Message, settings)
                    ))
                    .ToArray()
                , cancellationToken);
            position = appendResult.CurrentPosition + 1;
        }

        return position;
    }

    private async Task<IReadOnlyCollection<RecordedEvent>> ReadThens(long position, CancellationToken cancellationToken)
    {
        var recorded = new List<RecordedEvent>();
        var page     = await store.ReadAllForwards(position, 1024, cancellationToken: cancellationToken);
        foreach (var then in page.Messages)
        {
            var streamName = new StreamName(then.StreamId);
            var jsonData   = await then.GetJsonData(cancellationToken);
            var message = JsonSerializer.Deserialize(
                jsonData,
                messageTypeResolver(then.Type),
                settings
            )!;
            var recordedEvent = new RecordedEvent(streamName, message);
            recorded.Add(recordedEvent);
        }
        while (!page.IsEnd)
        {
            page = await page.ReadNext(cancellationToken);
            foreach (var then in page.Messages)
            {
                var streamName    = new StreamName(then.StreamId);
                var jsonData      = await then.GetJsonData(cancellationToken);
                var message = JsonSerializer.Deserialize(
                    jsonData,
                    messageTypeResolver(then.Type),
                    settings
                )!;
                var recodedEvent = new RecordedEvent(streamName, message);
                recorded.Add(recodedEvent);
            }
        }
        return recorded.ToArray();
    }
}