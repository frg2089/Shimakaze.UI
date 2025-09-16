using System.Runtime.InteropServices;

using Shimakaze.UI.Native.Cocoa.Interop;

namespace Shimakaze.UI.Native.Cocoa.Native;

internal static partial class ObjC
{
    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "class_getName", StringMarshalling = StringMarshalling.Utf8)]
    public static partial string GetName(Class cls);

    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "class_getSuperclass", StringMarshalling = StringMarshalling.Utf8)]
    public static partial Class GetSuperClass(Class cls);

    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "class_isMetaClass", StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool IsMetaClass(Class cls);

    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "class_getInstanceSize", StringMarshalling = StringMarshalling.Utf8)]
    public static partial uint GetInstanceSize(Class cls);

    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "class_getInstanceVariable", StringMarshalling = StringMarshalling.Utf8)]
    public static partial Ivar GetInstanceVariable(Class cls, string name);

    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "class_getClassVariable", StringMarshalling = StringMarshalling.Utf8)]
    public static partial Ivar GetClassVariable(Class cls, string name);

    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "class_addIvar", StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool AddVariable(Class cls, string name, nint size, byte alignment, string type);

    [LibraryImport(LibraryNames.LibObjC, EntryPoint = "class_getVersion", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int GetVersion(Class cls);
}