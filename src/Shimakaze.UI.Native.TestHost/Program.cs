
using Shimakaze.UI.Native;
using Shimakaze.UI.Native.Gtk4;
using Shimakaze.UI.Native.Gtk4.WebKitGtk6;
using Shimakaze.UI.Native.WebView;
using Shimakaze.UI.Native.Win32;
using Shimakaze.UI.Native.Win32.WebView2;

Dispatcher dispatcher = OperatingSystem.IsWindowsVersionAtLeast(5, 0)
    ? new Win32Dispatcher()
    : new Gtk4Dispatcher();

Application app = OperatingSystem.IsWindowsVersionAtLeast(5, 0)
    ? new Win32Application(dispatcher)
    : new Gtk4Application(dispatcher);

app.Initialize += (_, _) =>
{
    Window window = OperatingSystem.IsWindowsVersionAtLeast(5, 0)
        ? new Win32Window()
        : new Gtk4Window();
    WebView webview = OperatingSystem.IsWindowsVersionAtLeast(6, 1)
        ? new EdgeWebView2()
        : new WebKitGtk6();

    webview.Window = window;
    webview.NavigateTo("https://cn.bing.com");
    window.Show();
};
app.Run();
