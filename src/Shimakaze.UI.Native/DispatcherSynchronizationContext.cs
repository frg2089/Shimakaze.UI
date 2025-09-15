namespace Shimakaze.UI.Native;

public sealed class DispatcherSynchronizationContext(Dispatcher dispatcher) : SynchronizationContext
{
    public override void Send(SendOrPostCallback d, object? state)
    {
        if (dispatcher.CheckAccess())
            d(state);
        else
            dispatcher.InvokeAsync(DispatcherPriority.Normal, () => d(state)).GetAwaiter().GetResult();
    }

    public override void Post(SendOrPostCallback d, object? state)
        => dispatcher.InvokeAsync(DispatcherPriority.Normal, () => d(state));
}