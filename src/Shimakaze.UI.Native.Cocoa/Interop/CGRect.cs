using System.Drawing;
using System.Runtime.InteropServices;

namespace Shimakaze.UI.Native.Cocoa.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct CGRect(CGPoint origin, CGSize size)
{
    public static readonly CGRect Zero = default;

    public CGPoint Origin = origin;
    public CGSize Size = size;

    public CGRect(double x, double y, double width, double height)
        : this(new(x, y), new(width, height))
    {
    }

    public static implicit operator CGRect(Rectangle rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator Rectangle(CGRect rect) => new((int)rect.Origin.X, (int)rect.Origin.Y, (int)rect.Size.Width, (int)rect.Size.Height);

    public static implicit operator CGRect(RectangleF rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator RectangleF(CGRect rect) => new((float)rect.Origin.X, (float)rect.Origin.Y, (float)rect.Size.Width, (float)rect.Size.Height);
}
