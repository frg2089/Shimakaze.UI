using System.Runtime.InteropServices;

namespace Shimakaze.UI.Native.Cocoa.Interop;

internal abstract class ObjCSafeHandle(nint invalidHandleValue, bool ownsHandle = false) : SafeHandle(invalidHandleValue, ownsHandle), IEquatable<ObjCSafeHandle?>
{
    public override bool IsInvalid => handle is 0;

    protected override bool ReleaseHandle() => true;

    public override bool Equals(object? obj) => Equals(obj as ObjCSafeHandle);

    public bool Equals(ObjCSafeHandle? other) => other is not null && handle.Equals(other.handle);

    public override int GetHashCode() => HashCode.Combine(handle);

    public static bool operator ==(ObjCSafeHandle? left, ObjCSafeHandle? right) => EqualityComparer<ObjCSafeHandle>.Default.Equals(left, right);

    public static bool operator !=(ObjCSafeHandle? left, ObjCSafeHandle? right) => !(left == right);
}
