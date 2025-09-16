namespace Shimakaze.UI.Native.Cocoa.Interop;

internal sealed partial class NSWindow : NSObject
{
    private NSWindow(nint handle) : base(handle, false)
    {
    }

    public NSWindow() : this(0)
    {
    }

    [SendMessage("alloc")]
    public static partial NSWindow Alloc();

    [SendMessage("close")]
    public partial void CloseWindow(nint arg0);
    public void CloseWindow() => CloseWindow(0);

    [SendMessage("center")]
    public partial void Center();
    [SendMessage("makeKeyAndOrderFront:")]
    public partial void MakeKeyAndOrderFront(nint arg0);

    [SendMessage("initWithContentRect:styleMask:backing:defer:")]
    public partial void InitWithContentRect(CGRect rect, nuint style, nuint arg2, bool arg3);
}
