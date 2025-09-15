using System.Diagnostics;

namespace Shimakaze.UI.Native.Gtk4;

public sealed class Gtk4Dispatcher : Dispatcher
{
    protected override void Enqueue(IDispatcherTask task)
    {
        GLib.Functions.IdleAdd(
            task.Priority switch
            {
                DispatcherPriority.Idle => GLib.Constants.PRIORITY_DEFAULT_IDLE,
                DispatcherPriority.Low => GLib.Constants.PRIORITY_LOW,
                DispatcherPriority.Normal => GLib.Constants.PRIORITY_DEFAULT,
                DispatcherPriority.High => GLib.Constants.PRIORITY_HIGH,
                _ => GLib.Constants.PRIORITY_DEFAULT,
            },
            () =>
            {
                task.Invoke();
                return false;
            });
    }

    protected override void MainLoop()
    {
        var args = Environment.GetCommandLineArgs();
        var result = Gtk4Application.Instance.NativeApp.Run(args.Length, args);
        Debug.Assert(result is 0);
    }
}