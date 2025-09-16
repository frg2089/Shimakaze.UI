namespace Shimakaze.UI.Native.Cocoa.Interop;

internal sealed partial class NSApplication : NSObject
{
    [SendMessage("sharedApplication")]
    public static partial NSApplication SharedApplication { get; }

    private NSApplication(nint handle) : base(handle, false)
    {
    }

    public NSApplication() : this(0)
    {
    }

    [SendMessage("setActivationPolicy")]
    public partial void SetActivationPolicy(NSApplicationActivationPolicy policy);

    [SendMessage("setDelegate:")]
    public partial void SetDelegate(nint a);

    [SendMessage("terminate:")]
    public partial void Terminate(NSObject sender);
    public void Terminate() => Terminate(this);

    [SendMessage("run")]
    public partial void Run();
}
