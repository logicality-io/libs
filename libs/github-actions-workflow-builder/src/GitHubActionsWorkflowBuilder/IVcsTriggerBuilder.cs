namespace Logicality.GithubActionsWorkflowBuilder;

public interface IVcsTriggerBuilder : ITriggerBuilder
{
    public IVcsTriggerBuilder Branches(params string[] branches);

    public IVcsTriggerBuilder BranchesIgnore(params string[] branches);

    public IVcsTriggerBuilder Paths(params string[] paths);

    public IVcsTriggerBuilder PathsIgnore(params string[] paths);

    public IVcsTriggerBuilder Tags(params string[] tags);

    public IVcsTriggerBuilder TagsIgnore(params string[] tags);

}