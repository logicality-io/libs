namespace Logicality.LittleForker;

public static class ProcessSupervisorExtensions
{
    public static Task WhenStateIs(
        this ProcessSupervisor  processSupervisor,
        ProcessSupervisor.State processState,
        CancellationToken       cancellationToken = default)
    {
        var taskCompletionSource = new TaskCompletionSource<int>();
        cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());

        void Handler(ProcessSupervisor.State state)
        {
            if (processState == state)
            {
                taskCompletionSource.SetResult(0);
                processSupervisor.StateChanged -= Handler;
            }
        }

        processSupervisor.StateChanged += Handler;

        return taskCompletionSource.Task;
    }
}