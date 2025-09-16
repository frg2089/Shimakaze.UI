using System.Runtime.InteropServices;

namespace Shimakaze.UI.Native.Cocoa.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct CGSize(double width, double height)
{
    public double Width = width;
    public double Height = height;
}
