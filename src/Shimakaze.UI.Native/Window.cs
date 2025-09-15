using System.ComponentModel;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Shimakaze.UI.Native;

public abstract partial class Window : ObservableObject, IDisposable
{
    [ObservableProperty]
    public partial Window? Parent { get; protected internal set; }
    [ObservableProperty]
    public partial Rectangle Rectangle { get; set; }
    [ObservableProperty]
    public partial string? Title { get; set; }

    public event EventHandler? Activated;
    public event EventHandler? Closed;
    public event CancelEventHandler? Closing;
    public event EventHandler? Deactivated;

    protected Window()
    {
        Application.Instance.Windows.Add(this);
    }

    public abstract void Show();

    public abstract void Close();

    protected virtual void OnActivated() => Activated?.Invoke(this, EventArgs.Empty);
    protected virtual void OnClosed()
    {
        Closed?.Invoke(this, EventArgs.Empty);
        Application.Instance.Windows.Remove(this);
    }

    protected virtual void OnClosing(CancelEventArgs e) => Closing?.Invoke(this, e);
    protected virtual void OnDeactivated() => Deactivated?.Invoke(this, EventArgs.Empty);

    protected virtual void Dispose(bool disposing)
    {
    }


    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
