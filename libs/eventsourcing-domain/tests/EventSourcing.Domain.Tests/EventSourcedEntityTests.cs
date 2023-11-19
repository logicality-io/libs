using Shouldly;
using Xunit;

namespace Logicality.EventSourcing.Domain
{
    namespace EventSourcedEntityTests
    {
        public class EventSourcedEntityWithoutRecordedEvents
        {
            private readonly Entity _sut;

            public EventSourcedEntityWithoutRecordedEvents()
            {
                _sut = new Entity();
            }

            [Fact]
            public void RestoreFromEventsHasExpectedResult()
            {
                var events = new object[] { new Event(), new Event() };
                _sut.RestoreFromEvents(events);
                _sut.RestoredFromEvents.ShouldBe(events);
            }

            [Fact]
            public void SuccessiveRestoreFromEventsHasExpectedResult()
            {
                var events1 = new object[] { new Event(), new Event() };
                var events2 = new object[] { new Event(), new Event() };
                _sut.RestoreFromEvents(events1);
                _sut.RestoreFromEvents(events2);
                _sut.RestoredFromEvents.ShouldBe(events1.Concat(events2).ToArray());
            }

            [Fact]
            public void TakeEventsHasExpectedResult()
            {
                _sut.TakeEvents().ShouldBeEmpty();
            }

            [Fact]
            public void SuccessiveTakeEventsHasExpectedResult()
            {
                _sut.TakeEvents();
                _sut.TakeEvents().ShouldBeEmpty();
            }

            [Fact]
            public void ApplyHasExpectedResult()
            {
                var @event = new Event();
                _sut.Apply(@event);
                _sut.TakeEvents().ShouldBe(new object[] { @event });
            }

            private class Entity : EventSourcedEntity
            {
                private readonly List<object> _restoredFromEvents;

                public Entity()
                {
                    _restoredFromEvents = new List<object>();
                    On<Event>(e => _restoredFromEvents.Add(e));
                }

                public object[] RestoredFromEvents => _restoredFromEvents.ToArray();

                public void Apply(Event @event)
                {
                    base.Apply(@event);
                }
            }

            private class Event
            {
            }
        }

        public class EventSourcedEntityWithRecordedEvents
        {
            private readonly Entity _sut;
            private readonly object[] _recordedEvents;

            public EventSourcedEntityWithRecordedEvents()
            {
                _sut = new Entity();
                _sut.RestoreFromEvents(new object[] { new Event(), new Event() });
                _recordedEvents = new object[] { new Event(), new Event() };
                _sut.RecordEvents(_recordedEvents);
            }

            [Fact]
            public void RestoreFromEventsThrows()
            {
                var events = new object[] { new Event(), new Event() };
                Should.Throw<InvalidOperationException>(() => _sut.RestoreFromEvents(events));
            }

            [Fact]
            public void TakeEventsHasExpectedResult()
            {
                var result = _sut.TakeEvents();
                result.ShouldBe(_recordedEvents);
            }

            [Fact]
            public void SuccessiveTakeEventsHasExpectedResult()
            {
                _sut.TakeEvents();
                _sut.TakeEvents().ShouldBeEmpty();
            }

            [Fact]
            public void ApplyHasExpectedResult()
            {
                var @event = new Event();
                _sut.Apply(@event);

                _recordedEvents.Concat(new object[] { @event }).ToArray().ShouldBe(_sut.TakeEvents());
            }

            private class Entity : EventSourcedEntity
            {
                private readonly List<object> _restoredFromEvents;

                public Entity()
                {
                    _restoredFromEvents = new List<object>();
                    On<Event>(e => _restoredFromEvents.Add(e));
                }

                public object[] RestoredFromEvents => _restoredFromEvents.ToArray();

                public void Apply(Event @event)
                {
                    base.Apply(@event);
                }

                public void RecordEvents(IEnumerable<object> events)
                {
                    foreach (var @event in events)
                    {
                        Apply(@event);
                    }
                }
            }

            private class Event
            {
            }
        }
    }
}
