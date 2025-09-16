namespace Shimakaze.UI.Native.Cocoa.Interop;

internal sealed class Ivar(nint invalidHandleValue) : ObjCSafeHandle(invalidHandleValue, false)
{
    public Ivar() : this(0)
    {
    }
}