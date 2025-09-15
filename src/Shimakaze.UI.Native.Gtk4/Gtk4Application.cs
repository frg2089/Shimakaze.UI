namespace Shimakaze.UI.Native.Gtk4;

public sealed class Gtk4Application : Application
{
    private bool _disposedValue;

    internal readonly Gtk.Application NativeApp;

    public static new Gtk4Application Instance => (Gtk4Application)Application.Instance;

    public Gtk4Application(Gtk4Dispatcher dispatcher) : base(dispatcher)
    {
        Gtk.Module.Initialize();
        NativeApp = Gtk.Application.New(null, Gio.ApplicationFlags.DefaultFlags);
        NativeApp.OnActivate += (_, _) => OnInitialize();
    }

    public override void Shutdown() => NativeApp.Quit();


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_disposedValue)
            return;
        
        if (disposing)
            NativeApp.Dispose();
        
        _disposedValue = true;
    }
}
