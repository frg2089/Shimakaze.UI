using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Shimakaze.UI.Native.Resources;

namespace Shimakaze.UI.Native;

public abstract class Application : IDisposable
{
    private bool _disposedValue;

    public event EventHandler? Initialize;

    protected internal ObservableCollection<Window> Windows { get; } = [];

    public static Application Instance { get; private set; } = default!;

    public Dispatcher Dispatcher { get; }

    protected Application(Dispatcher dispatcher)
    {
        if (Instance is not null)
            throw new ApplicationException(Resource.SecondApplicationError);

        Instance = this;
        Dispatcher = dispatcher;
    }

    private void Windows_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Windows.Count is 0)
            Shutdown();
    }

    public void Start()
    {
        Windows.CollectionChanged += Windows_CollectionChanged;
        Dispatcher.UIThread.Start();
    }

    public abstract void Shutdown();

    public void WaitForExit() => Dispatcher.UIThread.Join();

    public void Run()
    {
        Start();
        WaitForExit();
    }

    protected virtual void OnInitialize() => Initialize?.Invoke(this, EventArgs.Empty);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
        {
            foreach (var window in Windows)
                window.Dispose();

            Windows.Clear();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
