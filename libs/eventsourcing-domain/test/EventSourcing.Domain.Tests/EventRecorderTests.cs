using System;
using Shouldly;
using Xunit;

namespace Logicality.EventSourcing.Domain.Tests
{
    namespace EventRecorderTests
    {
        public class EventRecorderWithoutRecordedEvents
        {
            private readonly EventRecorder _sut;

            public EventRecorderWithoutRecordedEvents()
            {
                _sut = new EventRecorder();
            }

            [Fact]
            public void HasRecordedEventsReturnsExpectedResult()
            {
                _sut.HasRecordedEvents.ShouldBeFalse();
            }

            [Fact]
            public void RecordedEventsReturnsExpectedResult()
            {
                _sut.RecordedEvents.ShouldBeEmpty();
            }

            [Fact]
            public void RecordNullEventHasExpectedResult()
            {
                Should.Throw<ArgumentNullException>(() => _sut.Record(null));
            }
            
            [Fact]
            public void RecordHasExpectedResult()
            {
                var @event = new object();
                _sut.Record(@event);

                _sut.HasRecordedEvents.ShouldBeTrue();
                _sut.RecordedEvents.ShouldBe(new[] { @event });
            }

            [Fact]
            public void ResetHasExpectedResult()
            {
                _sut.Reset();

                _sut.HasRecordedEvents.ShouldBeFalse();
                _sut.RecordedEvents.ShouldBeEmpty();
            }
        }

        public class EventRecorderWithRecordedEvents
        {
            private readonly EventRecorder _sut;
            private readonly object[] _recorded = {
                new object(), new object(),
            };

            public EventRecorderWithRecordedEvents()
            {
                _sut = new EventRecorder();
                foreach (var record in _recorded)
                {
                    _sut.Record(record);
                }
            }

            [Fact]
            public void HasRecordedEventsReturnsExpectedResult()
            {
                _sut.HasRecordedEvents.ShouldBeTrue();
            }

            [Fact]
            public void RecordedEventsReturnsExpectedResult()
            {
                _sut.RecordedEvents.ShouldBe(_recorded);
            }

            [Fact]
            public void RecordNullEventHasExpectedResult()
            {
                Should.Throw<ArgumentNullException>(() => _sut.Record(null));
            }

            [Fact]
            public void RecordHasExpectedResult()
            {
                var @event = new object();
                _sut.Record(@event);

                _sut.HasRecordedEvents.ShouldBeTrue();
                _sut.RecordedEvents.ShouldBe(new[] { _recorded[0], _recorded[1], @event });
            }

            [Fact]
            public void ResetHasExpectedResult()
            {
                _sut.Reset();

                _sut.HasRecordedEvents.ShouldBeFalse();
                _sut.RecordedEvents.ShouldBeEmpty();
            }
        }
    }
}
