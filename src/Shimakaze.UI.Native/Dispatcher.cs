using Shimakaze.UI.Native.Internal;

namespace Shimakaze.UI.Native;

/// <summary>
/// 调度器类，用于管理UI线程和操作调度
/// 提供同步和异步方法来执行操作，确保操作在正确的线程上下文中运行
/// </summary>
public abstract class Dispatcher
{
    internal readonly Thread UIThread;
    private readonly Lazy<DispatcherSynchronizationContext> _synchronizationContext;

    /// <summary>
    /// 获取与当前调度器关联的同步上下文
    /// </summary>
    public DispatcherSynchronizationContext DispatcherSynchronizationContext => _synchronizationContext.Value;

    /// <summary>
    /// 初始化调度器，创建并配置UI线程
    /// </summary>
    protected Dispatcher()
    {
        _synchronizationContext = new(() => new(this));
        UIThread = new(() =>
        {
            SynchronizationContext.SetSynchronizationContext(DispatcherSynchronizationContext);
            MainLoop();
        })
        {
            Name = "UI Thread",
            IsBackground = false,
        };

        if (OperatingSystem.IsWindows())
            UIThread.SetApartmentState(ApartmentState.STA);
    }

    /// <summary>
    /// 检查当前线程是否为UI线程
    /// </summary>
    /// <returns>如果当前线程是UI线程返回true，否则返回false</returns>
    internal bool CheckAccess() => UIThread == Thread.CurrentThread;

    /// <summary>
    /// 将操作加入调度队列
    /// </summary>
    /// <param name="priority">调度优先级</param>
    /// <param name="handler">要执行的操作</param>
    protected abstract void Enqueue(IDispatcherTask task);

    /// <summary>
    /// 主循环逻辑，由派生类实现
    /// </summary>
    protected abstract void MainLoop();

    /// <summary>
    /// 在 UI 线程上执行操作
    /// </summary>
    /// <param name="priority">调度优先级</param>
    /// <param name="action">要执行的操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    public IDispatcherTask InvokeAsync(DispatcherPriority priority, Action action, CancellationToken cancellationToken = default)
        => InvokeAsync(priority, _ => action(), cancellationToken);

    /// <summary>
    /// 在 UI 线程上执行操作
    /// </summary>
    /// <param name="priority">调度优先级</param>
    /// <param name="action">要执行的操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    public IDispatcherTask InvokeAsync(DispatcherPriority priority, Action<CancellationToken> action, CancellationToken cancellationToken = default)
    {
        DispatcherTask task = new(priority, action, cancellationToken);
        Enqueue(task);
        return task;
    }

    /// <summary>
    /// 在 UI 线程上执行操作
    /// </summary>
    /// <param name="priority">调度优先级</param>
    /// <param name="action">要执行的操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    public IDispatcherTask<TResult> InvokeAsync<TResult>(DispatcherPriority priority, Func<TResult> action, CancellationToken cancellationToken = default)
        => InvokeAsync(priority, _ => action(), cancellationToken);

    /// <summary>
    /// 在 UI 线程上执行操作
    /// </summary>
    /// <param name="priority">调度优先级</param>
    /// <param name="action">要执行的操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    public IDispatcherTask<TResult> InvokeAsync<TResult>(DispatcherPriority priority, Func<CancellationToken, TResult> action, CancellationToken cancellationToken = default)
    {
        DispatcherTask<TResult> task = new(priority, action, cancellationToken);
        Enqueue(task);
        return task;
    }
}
