using System.Collections.Immutable;

namespace Logicality.EventSourcing.Domain.Testing;

public class Scenario :
    Scenario.IScenarioInitialStateBuilder,
    Scenario.IScenarioGivenNoEventsStateBuilder,
    Scenario.IScenarioGivenEventsStateBuilder,
    Scenario.IScenarioWhenStateBuilder,
    Scenario.IScenarioThenEventsStateBuilder,
    Scenario.IScenarioThenNoEventsStateBuilder,
    Scenario.IScenarioBuilder
{
    private readonly ImmutableList<RecordedEvent> _givens;

    private readonly object _when;

    private readonly ImmutableList<RecordedEvent> _thens;

    private readonly object _result;

    public Scenario()
        : this(ImmutableList<RecordedEvent>.Empty, null, ImmutableList<RecordedEvent>.Empty, null)
    {
    }

    private Scenario(ImmutableList<RecordedEvent> givens, object when, ImmutableList<RecordedEvent> thens, object result)
    {
        _givens = givens;
        _when   = when;
        _thens  = thens;
        _result = result;
    }

    public IReadOnlyCollection<RecordedEvent> Givens => _givens.ToArray();

    public object When => _when;
        
    public IReadOnlyCollection<RecordedEvent> Thens => _thens.ToArray();

    public object Result => _result;
        
    public IScenarioGivenNoEventsStateBuilder GivenNone()
    {
        return this;
    }
        
    public IScenarioGivenEventsStateBuilder Given(params RecordedEvent[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        return new Scenario(_givens.AddRange(events), _when, _thens, _result);
    }
        
    public IScenarioGivenEventsStateBuilder Given(StreamName stream, params object[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        return new Scenario(_givens.AddRange(Transform(stream, events)), _when, _thens, _result);
    }

    IScenarioGivenEventsStateBuilder IScenarioGivenEventsStateBuilder.Given(StreamName stream, params object[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        return new Scenario(_givens.AddRange(Transform(stream, events)), _when, _thens, _result);
    }

    IScenarioGivenEventsStateBuilder IScenarioGivenEventsStateBuilder.Given(params RecordedEvent[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        return new Scenario(_givens.AddRange(events), _when, _thens, _result);
    }

    IScenarioWhenStateBuilder IScenarioGivenEventsStateBuilder.When(object command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return new Scenario(_givens, command, _thens, _result);
    }

    IScenarioWhenStateBuilder IScenarioGivenNoEventsStateBuilder.When(object command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        return new Scenario(_givens, command, _thens, _result);
    }

    IScenarioThenNoEventsStateBuilder IScenarioWhenStateBuilder.ThenNone()
    {
        return this;
    }

    IScenarioThenEventsStateBuilder IScenarioWhenStateBuilder.Then(params RecordedEvent[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        return new Scenario(_givens, _when, _thens.AddRange(events), _result);
    }

    IScenarioThenEventsStateBuilder IScenarioThenEventsStateBuilder.Then(StreamName stream, params object[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }
            
        return new Scenario(_givens, _when, _thens.AddRange(Transform(stream, events)), _result);
    }

    IScenarioThenEventsStateBuilder IScenarioWhenStateBuilder.Then(StreamName stream, params object[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }
            
        return new Scenario(_givens, _when, _thens.AddRange(Transform(stream, events)), _result);
    }

    IScenarioThenEventsStateBuilder IScenarioThenEventsStateBuilder.Then(params RecordedEvent[] events)
    {
        if (events == null)
        {
            throw new ArgumentNullException(nameof(events));
        }

        return new Scenario(_givens, _when, _thens.AddRange(events), _result);
    }

    IScenarioBuilder IScenarioThenEventsStateBuilder.Expect(object outcome)
    {
        if (outcome == null)
        {
            throw new ArgumentNullException(nameof(outcome));
        }

        return new Scenario(_givens, _when, _thens, outcome);
    }

    IScenarioBuilder IScenarioThenNoEventsStateBuilder.Expect(object outcome)
    {
        if (outcome == null)
        {
            throw new ArgumentNullException(nameof(outcome));
        }

        return new Scenario(_givens, _when, _thens, outcome);
    }

    Scenario IScenarioBuilder.Build()
    {
        return this;
    }
        
    public ScenarioPassed Pass(object result) => new ScenarioPassed(this);

    public ScenarioRecordedOtherEvents ButRecordedOtherEvents(IReadOnlyCollection<RecordedEvent> other) =>
        new ScenarioRecordedOtherEvents(this, other);

    public ScenarioReturnedOtherResult ButReturnedOtherResult(object other) =>
        new ScenarioReturnedOtherResult(this, other);
        
    public ScenarioThrewException ButThrewException(Exception exception) =>
        new ScenarioThrewException(this, exception);

    private static IEnumerable<RecordedEvent> Transform(StreamName stream, object[] events)
    {
        return Array.ConvertAll(events, message => new RecordedEvent(stream, message));
    }
        
    public interface IScenarioBuilder
    {
        Scenario Build();
    }
        
    public interface IScenarioGivenEventsStateBuilder
    {
        //TODO: May want to get rid of the RecordedEvent overloads since they only confuse and cause two syntax's
            
        IScenarioGivenEventsStateBuilder Given(params RecordedEvent[] events);
        
        IScenarioGivenEventsStateBuilder Given(StreamName stream, params object[] events);

        IScenarioWhenStateBuilder When(object command);
    }
        
    public interface IScenarioGivenNoEventsStateBuilder
    {
        IScenarioWhenStateBuilder When(object command);
    }
        
    public interface IScenarioInitialStateBuilder
    {
        IScenarioGivenNoEventsStateBuilder GivenNone();
        
        IScenarioGivenEventsStateBuilder Given(params RecordedEvent[] events);
        
        IScenarioGivenEventsStateBuilder Given(StreamName stream, params object[] events);
    }
        
    public interface IScenarioThenEventsStateBuilder
    {
        IScenarioThenEventsStateBuilder Then(params RecordedEvent[] events);
        
        IScenarioThenEventsStateBuilder Then(StreamName stream, params object[] events);

        IScenarioBuilder Expect(object result);        
    }
        
    public interface IScenarioThenNoEventsStateBuilder
    {
        IScenarioBuilder Expect(object result);
    }
        
    public interface IScenarioWhenStateBuilder
    {
        IScenarioThenNoEventsStateBuilder ThenNone();
        
        IScenarioThenEventsStateBuilder Then(params RecordedEvent[] events);
        
        IScenarioThenEventsStateBuilder Then(StreamName stream, params object[] events);
    }
}