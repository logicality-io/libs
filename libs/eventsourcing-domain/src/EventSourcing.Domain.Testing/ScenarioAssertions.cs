using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;

namespace Logicality.EventSourcing.Domain.Testing;

public static class ScenarioAssertions
{
    private static readonly JsonSerializerOptions SerializerSettings;

    static ScenarioAssertions()
    {
        SerializerSettings = new JsonSerializerOptions //TODO IsoDateFormat
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented        = true,
        };
        SerializerSettings.Converters.Add(new JsonStringEnumConverter());
    }

    public static async Task AssertAsync(
        this Scenario.IScenarioBuilder builder,
        ScenarioRunner                 runner,
        CancellationToken              ct = default)
    {
        var messageBuilder = new StringBuilder();
        var config = new ComparisonConfig
        {
            MaxDifferences = int.MaxValue,
            MaxStructDepth = 5,
        };
        var comparer = new CompareLogic(config);

        var scenario = builder.Build();
        var outcome  = await runner.RunAsync(scenario, ct);
        if (outcome is ScenarioPassed)
        {
            return;
        }

        switch (outcome)
        {
            case ScenarioThrewException threw:
                messageBuilder.Append($"Scenario threw unexpectedly {threw.Exception}");
                break;
            case ScenarioRecordedOtherEvents recorded:
                if (recorded.Scenario.Thens.Count != recorded.Actual.Count)
                {
                    messageBuilder.AppendFormat("Expected {0} events ({1}) but recorded {2} events ({3}).",
                        recorded.Scenario.Thens.Count,
                        string.Join(",",
                            recorded.Scenario.Thens.Select(given =>
                                $"[Stream={given.Stream}]Event={given.Message.GetType().Name}")),
                        recorded.Actual.Count,
                        string.Join(",",
                            recorded.Actual.Select(actual =>
                                $"[Stream={actual.Stream}]Event={actual.Message.GetType().Name}")));
                }
                else
                {
                    messageBuilder.AppendLine("Expected events to match but found differences:");

                    var eventComparison = comparer.Compare(recorded.Scenario.Thens, recorded.Actual);
                    foreach (var difference in eventComparison.Differences)
                    {
                        messageBuilder.AppendLine("\t" + difference);
                    }

                    messageBuilder.AppendLine("Expected:");
                    foreach (var then in recorded.Scenario.Thens)
                    {
                        messageBuilder.AppendLine($"[Stream={then.Stream}]Event={then.Message.GetType().Name}");
                        messageBuilder.AppendLine(JsonSerializer.Serialize(then.Message, SerializerSettings));
                    }

                    messageBuilder.AppendLine("Actual:");
                    foreach (var actual in recorded.Actual)
                    {
                        messageBuilder.AppendLine($"[Stream={actual.Stream}]Event={actual.Message.GetType().Name}");
                        messageBuilder.AppendLine(JsonSerializer.Serialize(actual.Message,SerializerSettings));
                    }
                }

                break;
            case ScenarioReturnedOtherResult resulted:
                messageBuilder.AppendLine("Expected results to match but found differences:");
                var comparison = comparer.Compare(resulted.Scenario.Result, resulted.Actual);
                foreach (var difference in comparison.Differences)
                {
                    messageBuilder.AppendLine("\t" + difference);
                }

                break;
        }
        throw new Exception(messageBuilder.ToString());
        //messageBuilder.ToString().ShouldBe("");
    }
}