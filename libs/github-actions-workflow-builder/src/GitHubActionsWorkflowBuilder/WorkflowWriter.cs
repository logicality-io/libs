using System.Text;
using Logicality.GitHubActionsWorkflowBuilder;

namespace Logicality.GithubActionsWorkflowBuilder;

public class WorkflowWriter
{
    private          int           _indentCount;
    private readonly StringBuilder _stringBuilder = new();

    public WorkflowWriter WriteLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            _stringBuilder.AppendLine();
            return this;
        }
        _stringBuilder.Append(new string(' ', _indentCount * 2));
        _stringBuilder.AppendLine(line);
        return this;
    }

    public IDisposable Indent()
    {
        _indentCount++;
        return new DelegateDisposable(() => _indentCount--);
    }

    public override string ToString() => _stringBuilder.ToString();
}