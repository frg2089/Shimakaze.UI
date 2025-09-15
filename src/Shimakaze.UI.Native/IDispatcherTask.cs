using System.ComponentModel;

using Shimakaze.UI.Native.Internal;

namespace Shimakaze.UI.Native;

public interface IDispatcherTask
{
    DispatcherPriority Priority { get; }
    CancellationToken CancellationToken { get; }
    internal void Invoke();
    IDispatcherTaskAwaiter GetAwaiter();

    [EditorBrowsable(EditorBrowsableState.Never)]
    static void Invoke(IDispatcherTask task) => task.Invoke();
}

public interface IDispatcherTask<out TResult> : IDispatcherTask
{
    new IDispatcherTaskAwaiter<TResult> GetAwaiter();
    IDispatcherTaskAwaiter IDispatcherTask.GetAwaiter() => GetAwaiter();
}
