using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Shimakaze.UI.Native.Win32;

public sealed class Win32Dispatcher : Dispatcher
{
    private readonly ConcurrentDictionary<DispatcherPriority, ConcurrentQueue<IDispatcherTask>> _tasks = [];
    private uint _threadId = 0;
    internal const uint WM_TASK = PInvoke.WM_USER + 1;

    private volatile nint _loopCount = 0;

    protected override void Enqueue(IDispatcherTask task)
    {
        if (!_tasks.TryGetValue(task.Priority, out var queue))
            _tasks[task.Priority] = queue = [];

        queue.Enqueue(task);

        if (!PInvoke.PostThreadMessage(_threadId, WM_TASK, 0, 0))
            throw new Win32Exception();
    }

    private bool TryDequeue(DispatcherPriority priority, [NotNullWhen(true)] out IDispatcherTask? task)
    {
        if (!_tasks.TryGetValue(priority, out var queue))
        {
            task = null;
            return false;
        }

        return queue.TryDequeue(out task);
    }

    protected override void MainLoop()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
            _threadId = PInvoke.GetCurrentThreadId();

        Win32Application.Instance.OnInitialize();
        while (true)
        {
            if (!PInvoke.PeekMessage(out var msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE))
            {
                if (TryDequeue(DispatcherPriority.High, out var task)
                    || TryDequeue(DispatcherPriority.Normal, out task)
                    || TryDequeue(DispatcherPriority.Low, out task)
                    || TryDequeue(DispatcherPriority.Idle, out task))
                {
                    // 没有消息时尝试调用闲置任务
                    task.Invoke();
                }
                else
                {
                    // 没任务可执行时等待下一个消息
                    if (!PInvoke.WaitMessage())
                        throw new Win32Exception();
                }

                continue;
            }

            if (msg.message is PInvoke.WM_QUIT)
            {
                // Flush All Tasks
                while (TryDequeue(DispatcherPriority.High, out var task)
                    || TryDequeue(DispatcherPriority.Normal, out task)
                    || TryDequeue(DispatcherPriority.Low, out task)
                    || TryDequeue(DispatcherPriority.Idle, out task))
                    // 没有消息时尝试调用闲置任务
                    task.Invoke();

                break;
            }
            else if (msg.message is WM_TASK)
            {
                if (TryDequeue(DispatcherPriority.High, out var task))
                    // 没有消息时尝试调用闲置任务
                    task.Invoke();
            }
            else
            {
                if ((_loopCount & 0b1000) is not 0)
                {
                    if (TryDequeue(DispatcherPriority.Normal, out var task))
                        task.Invoke();
                }
                else if ((_loopCount & 0b10000) is not 0)
                {
                    if (TryDequeue(DispatcherPriority.Low, out var task))
                        task.Invoke();
                }
            }

            PInvoke.TranslateMessage(msg);
            PInvoke.DispatchMessage(msg);
            unchecked
            {
                _loopCount++;
            }
        }
    }
}
