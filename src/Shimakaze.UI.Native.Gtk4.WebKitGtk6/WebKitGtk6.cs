using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Shimakaze.UI.Native.Gtk4.WebKitGtk6;

public sealed class WebKitGtk6 : WebView.WebView
{
    private readonly WebKit.WebView _native;
    public WebKitGtk6()
    {
        PropertyChanged += WebKitGtk6WebView_RectangleChanged;
        PropertyChanged += WebKitGtk6WebView_WindowChangedd;
        WebKit.Module.Initialize();
        _native = WebKit.WebView.New();
    }

    private Gtk4Window GetNativeWindow()
    {
        if (Window is not Gtk4Window window)
            throw new Exception("WebKitGtk6 can only be assign in Gtk4Window");

        return window;
    }

    private void WebKitGtk6WebView_RectangleChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(Rectangle))
            return;

        try
        {
            PropertyChanged -= WebKitGtk6WebView_RectangleChanged;
            var rect = Rectangle;
            ref var rRect = ref rect;

            if (rRect.Width is 0)
                rRect.Width = Window!.Rectangle.Width;
            if (rRect.Height is 0)
                rRect.Height = Window!.Rectangle.Height;
            Rectangle = rRect;
            _native.SetSizeRequest(rRect.Width, rRect.Height);
        }
        finally
        {
            PropertyChanged += WebKitGtk6WebView_RectangleChanged;
        }
    }
    private void WebKitGtk6WebView_WindowChangedd(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(Window))
            return;

        GetNativeWindow().Native.Child = _native;
        try
        {
            PropertyChanged -= WebKitGtk6WebView_RectangleChanged;
            var rect = Rectangle;
            ref var rRect = ref rect;

            if (rRect.Width is 0)
                rRect.Width = Window!.Rectangle.Width;
            if (rRect.Height is 0)
                rRect.Height = Window!.Rectangle.Height;
            Rectangle = rRect;
            _native.SetSizeRequest(rRect.Width, rRect.Height);
        }
        finally
        {
            PropertyChanged += WebKitGtk6WebView_RectangleChanged;
        }
    }

    public override void NavigateTo([StringSyntax("Uri")] string uri) => _native.LoadUri(uri);
}
