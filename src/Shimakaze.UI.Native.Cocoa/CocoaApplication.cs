using Shimakaze.UI.Native.Cocoa.Interop;

namespace Shimakaze.UI.Native.Cocoa;

public sealed class CocoaApplication : Application
{
    internal readonly NSApplication Native;

    public static new CocoaApplication Instance => (CocoaApplication)Application.Instance;

    public CocoaApplication(Dispatcher dispatcher) : base(dispatcher)
    {
        Native = NSApplication.SharedApplication;
        Native.SetActivationPolicy(0);
        Native.SetDelegate(0 - 1);
    }

    public override void Shutdown()
    {
        Native.Terminate();
    }
}

public sealed class CocoaDispatcher : Dispatcher
{
    protected override void Enqueue(IDispatcherTask task)
    {
        throw new NotImplementedException();
    }

    protected override void MainLoop() => CocoaApplication.Instance.Native.Run();
}