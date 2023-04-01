namespace Logicality.WebhookRelay;

public static class WebhookRelayClientExtensions
{
    public static async Task WaitForState(this WebhookRelayClient client, ClientState clientState, TimeSpan timeout)
    {
        var tcs = new TaskCompletionSource<bool>();
        client.StateChanged += (_, state) =>
        {
            if (state == ClientState.HandlingMessages)
            {
                tcs.SetResult(true);
            }
        };

        if (await Task.WhenAny(tcs.Task, Task.Delay(timeout)) == tcs.Task)
        {
            return;
        }

        throw new TimeoutException();
    }
}