using System.ComponentModel;

namespace Shimakaze.UI.Native.Gtk4;

public sealed class Gtk4Window : Window
{
    internal Gtk.Window Native { get; private set; }
    private bool _disposedValue;

    public Gtk4Window() : base()
    {
        Native = Gtk.Window.New();
        Gtk4Application.Instance.NativeApp.AddWindow(Native);
        if (Parent is Gtk4Window { Native: { } parent })
            Native.SetParent(parent);
        Native.SetTitle(Title);
        Native.SetDefaultSize(Rectangle.Width, Rectangle.Height);

        Native.OnShow += (_, _) => OnActivated();
        Native.OnHide += (_, _) => OnDeactivated();
        Native.OnCloseRequest += (_, _) =>
        {
            CancelEventArgs args = new();
            OnClosing(args);
            return args.Cancel;
        };
        Native.OnDestroy += (_, _) => OnClosed();

    }

    protected override void OnClosed()
    {
        base.OnClosed();
        Gtk4Application.Instance.NativeApp.RemoveWindow(Native);
    }

    public override void Close() => Native.Close();

    public override void Show() => Native.Show();

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_disposedValue)
            return;

        if (disposing)
            Native.Dispose();

        _disposedValue = true;
    }
}