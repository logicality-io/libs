namespace Logicality.GitHub.Actions.Workflow;

internal static class EnumerableExtensions
{
    internal static bool None<TSource>(this IEnumerable<TSource>? source) 
        => source == null || !source.Any();
}
