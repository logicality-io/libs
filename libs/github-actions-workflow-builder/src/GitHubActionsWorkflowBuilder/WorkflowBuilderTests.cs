namespace Logicality.GithubActionsWorkflowBuilder;

public class WorkflowBuilder : IWorkflowBuilder
{
    private string _name;

    public WorkflowBuilder(string name)
    {
        _name = name;
    }

    public string Generate()
    {

    }
}

public interface IWorkflowBuilder
{
    public ITriggerBuilder OnPullRequest() 
        => new TriggerBuilder("pull_request", this);

    private class TriggerBuilder : ITriggerBuilder
    {
        public TriggerBuilder(string eventName, IWorkflowBuilder workflowBuilder)
        {
            WorkflowBuilder = workflowBuilder;
        }

        public IWorkflowBuilder WorkflowBuilder { get; }
    }
}

public interface ITriggerBuilder
{
    IWorkflowBuilder WorkflowBuilder { get; }
}

internal class Writer
{

    public IDisposable Indent()
    {
            return DelegateDisposable.CreateBracket(
                () => _indentation++,
                () => _indentation--);
    }
}
