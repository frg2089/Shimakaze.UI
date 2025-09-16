using System.Runtime.InteropServices;

namespace Shimakaze.UI.Native.Cocoa.Native;

internal static partial class ObjC
{
    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "objc_getClass", StringMarshalling = StringMarshalling.Utf8)]
    public static partial nint GetClass(string name);

    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "sel_registerName", StringMarshalling = StringMarshalling.Utf8)]
    public static partial nint RegisterName(string? name);
}
