namespace Shimakaze.UI.Native.Cocoa.Interop;

internal sealed class SEL(nint invalidHandleValue) : ObjCSafeHandle(invalidHandleValue, false)
{
    public SEL() : this(0)
    {
    }

    public SEL(string name) : this(Native.ObjC.RegisterName(name))
    {
    }

    public static implicit operator SEL(string name) => new(name);
}