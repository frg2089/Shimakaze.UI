namespace Shimakaze.UI.Native.Internal;

internal class DispatcherTask : IDispatcherTask
{
    private readonly DispatcherTaskAwaiter _awaiter;
    private readonly Action<CancellationToken> _action;
    private protected readonly SemaphoreSlim _semaphore = new(1, 1);
    private protected DispatcherTaskStatus _status = DispatcherTaskStatus.WaitingToRun;
    private protected Exception? _exception;

    private event Action? Completed;

    public DispatcherTask(DispatcherPriority priority, Action<CancellationToken> action, CancellationToken cancellationToken)
    {
        Priority = priority;
        _action = action;
        CancellationToken = cancellationToken;
        _awaiter = new(this);
        CancellationToken.Register(() =>
        {
            if (_status is not DispatcherTaskStatus.Completed and not DispatcherTaskStatus.Faulted)
                _status = DispatcherTaskStatus.Canceled;
        });
    }

    public DispatcherPriority Priority { get; }

    public CancellationToken CancellationToken { get; }


    public virtual void Invoke()
    {
        if (_status is not DispatcherTaskStatus.WaitingToRun)
            return;

        _semaphore.Wait(CancellationToken);
        try
        {
            _status = DispatcherTaskStatus.Running;
            _action(CancellationToken);
            _status = DispatcherTaskStatus.Completed;
        }
        catch (Exception ex)
        {
            _exception = ex;
            _status = DispatcherTaskStatus.Faulted;
        }
        finally
        {
            _semaphore.Release();
        }

        OnCompleted();
    }

    private protected void OnCompleted() => Completed?.Invoke();

    public IDispatcherTaskAwaiter GetAwaiter() => _awaiter;

    public class DispatcherTaskAwaiter(DispatcherTask task) : IDispatcherTaskAwaiter
    {
        public bool IsCompleted => task._status is DispatcherTaskStatus.Completed or DispatcherTaskStatus.Faulted or DispatcherTaskStatus.Canceled;

        public void GetResult()
        {
            if (task._status is DispatcherTaskStatus.WaitingToRun)
                task.Invoke();

            task._semaphore.Wait(task.CancellationToken);

            if (task._status is DispatcherTaskStatus.Canceled)
                task.CancellationToken.ThrowIfCancellationRequested();

            if (task is { _status: DispatcherTaskStatus.Faulted, _exception: { } ex })
                throw ex;

            if (task is { _status: DispatcherTaskStatus.Completed })
                return;

            throw new Exception("Invalid task state");
        }

        public void OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

        public void UnsafeOnCompleted(Action continuation)
        {
            if (IsCompleted)
                continuation();
            else
                task.Completed += continuation;
        }
    }
}

internal sealed class DispatcherTask<TResult> : DispatcherTask, IDispatcherTask<TResult>
{
    private readonly DispatcherTaskAwaiter _awaiter;
    private readonly Func<CancellationToken, TResult> _action;
    private TResult? _result;

    public DispatcherTask(DispatcherPriority priority, Func<CancellationToken, TResult> action, CancellationToken cancellationToken)
        : base(priority, ct => action(ct), cancellationToken)
    {
        _action = action;
        _awaiter = new(this);
    }

    public override void Invoke()
    {
        if (_status is not DispatcherTaskStatus.WaitingToRun)
            return;

        _semaphore.Wait(CancellationToken);
        try
        {
            _status = DispatcherTaskStatus.Running;
            _result = _action(CancellationToken);
            _status = DispatcherTaskStatus.Completed;
        }
        catch (Exception ex)
        {
            _exception = ex;
            _status = DispatcherTaskStatus.Faulted;
        }
        finally
        {
            _semaphore.Release();
        }

        OnCompleted();
    }

    public new IDispatcherTaskAwaiter<TResult> GetAwaiter() => _awaiter;

    public new sealed class DispatcherTaskAwaiter(DispatcherTask<TResult> task) : DispatcherTask.DispatcherTaskAwaiter(task), IDispatcherTaskAwaiter<TResult>
    {
        public new TResult GetResult()
        {
            if (task._status is DispatcherTaskStatus.WaitingToRun)
                task.Invoke();

            task._semaphore.Wait(task.CancellationToken);

            if (task._status is DispatcherTaskStatus.Canceled)
                task.CancellationToken.ThrowIfCancellationRequested();

            if (task is { _status: DispatcherTaskStatus.Faulted, _exception: { } ex })
                throw ex;

            if (task is { _status: DispatcherTaskStatus.Completed, _result: { } result })
                return result;

            throw new Exception("Invalid task state");
        }
    }
}