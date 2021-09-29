using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using SqlStreamStore;
using SqlStreamStore.Streams;

namespace Logicality.EventSourcing.Domain.Testing
{
    public class ScenarioRunner
    {
        private readonly IStreamStore _store;
        private readonly MessageNameResolver _messageNameResolver;
        private readonly MessageTypeResolver _messageTypeResolver;
        private readonly JsonSerializerOptions _options;
        private readonly Func<object, CancellationToken, Task<object>> _dispatcher;

        public ScenarioRunner(
            IStreamStore store,
            MessageNameResolver messageNameResolver,
            MessageTypeResolver messageTypeResolver,
            JsonSerializerOptions settings,
            Func<object, CancellationToken, Task<object>> dispatcher)
        {
            _store = store;
            _messageNameResolver = messageNameResolver;
            _messageTypeResolver = messageTypeResolver;
            _options = settings;
            _dispatcher = dispatcher;
        }
        
        public async Task<object> RunAsync(Scenario scenario, CancellationToken cancellationToken = default)
        {
            var position = await WriteGivens(scenario.Givens, cancellationToken);
            
            try
            {
                var result = await _dispatcher(scenario.When, cancellationToken);
                var thens = await ReadThens(position, cancellationToken);
                var config = new ComparisonConfig
                {
                    MaxDifferences = int.MaxValue,
                    MaxStructDepth = 5,
                };
                var comparer = new CompareLogic(config);
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
                var appendResult = await _store.AppendToStream(
                    new StreamId(stream.Key.ToString()),
                    ExpectedVersion.NoStream,
                    stream
                        .Select(@event => new NewStreamMessage(
                            Guid.NewGuid(),
                            _messageNameResolver(@event.Message.GetType()),
                            JsonSerializer.Serialize(@event.Message, _options)
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
            var page = await _store.ReadAllForwards(position, 1024, cancellationToken: cancellationToken);
            foreach (var then in page.Messages)
            {
                recorded.Add(
                    new RecordedEvent(
                        new StreamName(then.StreamId),
                        JsonSerializer.Deserialize(
                            await then.GetJsonData(cancellationToken),
                            _messageTypeResolver(then.Type),
                            _options
                        )
                    )
                );
            }
            while (!page.IsEnd)
            {
                page = await page.ReadNext(cancellationToken);
                foreach (var then in page.Messages)
                {
                    recorded.Add(
                        new RecordedEvent(
                            new StreamName(then.StreamId),
                            JsonSerializer.Deserialize(
                                await then.GetJsonData(cancellationToken),
                                _messageTypeResolver(then.Type),
                                _options
                            )
                        )
                    );
                }
            }
            return recorded.ToArray();
        }
    }
}
