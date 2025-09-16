using System.Runtime.InteropServices.Marshalling;

namespace Shimakaze.UI.Native.Cocoa.Interop;

internal sealed partial class Class(nint handle) : ObjCSafeHandle(handle)
{
    public string Name => Native.ObjC.GetName(this);
    public Class SuperClass => Native.ObjC.GetSuperClass(this);
    public bool IsMetaClass => Native.ObjC.IsMetaClass(this);
    public uint InstanceSize => Native.ObjC.GetInstanceSize(this);
    public int Version => Native.ObjC.GetVersion(this);

    public Class() : this(0)
    {
    }

    public Class(string className) : this(Native.ObjC.GetClass(className))
    {
    }

    [SendMessage("new")]
    internal partial nint CreateInstance();

    public Ivar GetInstanceVariable(string name) => Native.ObjC.GetInstanceVariable(this, name);
    public Ivar GetClassVariable(string name) => Native.ObjC.GetClassVariable(this, name);
    public bool AddVariable(string name, nint size, byte alignment, string type) => Native.ObjC.AddVariable(this, name, size, alignment, type);
}
