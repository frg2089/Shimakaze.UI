using System.Runtime.InteropServices;

namespace Shimakaze.UI.Native.Cocoa.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct CGPoint(double x, double y)
{
    public double X = x;
    public double Y = y;
}