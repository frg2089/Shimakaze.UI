using Shimakaze.UI.Native.Cocoa.Interop;

namespace Shimakaze.UI.Native.Cocoa;

public sealed class CocoaWindow : Window
{
    private readonly NSWindow _native;

    public CocoaWindow()
    {
        _native = NSWindow.Alloc();
        _native.InitWithContentRect(
            Rectangle,
            0,
            2,
            false);

        //ObjC.Call(Handle, "setDelegate:", windowDelegate.Handle);
    }

    public override void Close() => _native.Close();

    public override void Show()
    {
        _native.Center();
        _native.MakeKeyAndOrderFront(0);

        //MacApplication.SynchronizationContext.Post(s => Shown?.Invoke(this, EventArgs.Empty), null);
    }
}
