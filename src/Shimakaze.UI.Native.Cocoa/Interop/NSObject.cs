using System.Runtime.InteropServices.Marshalling;

namespace Shimakaze.UI.Native.Cocoa.Interop;

internal abstract partial class NSObject(nint invalidHandleValue, bool ownsHandle = false) : ObjCSafeHandle(invalidHandleValue, ownsHandle)
{
    public static Class ClassOf<T>() where T : NSObject
        => new(typeof(T).Name);

    public static T CreateInstance<T>()
        where T : NSObject, new()
    {
        bool isSuccess = false;
        SafeHandleMarshaller<T>.ManagedToUnmanagedOut marshaller = new();
        try
        {
            var retVal = ClassOf<T>().CreateInstance();
            isSuccess = true;
            marshaller.FromUnmanaged(retVal);
            return marshaller.ToManaged();
        }
        finally
        {
            if (isSuccess)
                marshaller.Free();
        }
    }

    public Class GetClass() => new(GetType().Name);

}
