using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using Microsoft.Web.WebView2.Core;

namespace Shimakaze.UI.Native.Win32.WebView2;

public sealed class EdgeWebView2 : WebView.WebView
{
    private NativeWrapper? _native;

    public EdgeWebView2()
    {
        PropertyChanged += EdgeWebView2_RectangleChanged;
        PropertyChanged += EdgeWebView2_WindowChanged;
    }

    [MemberNotNull(nameof(_native))]
    private NativeWrapper GetNative()
    {
        if (Window is not Win32Window window)
            throw new Exception("EdgeWebView2 can only be assign in Win32Window");

        _native ??= new(window);
        return _native;
    }

    private async void EdgeWebView2_RectangleChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(Rectangle))
            return;

        try
        {
            PropertyChanged -= EdgeWebView2_RectangleChanged;
            var native = GetNative();
            var controller = await native.GetControllerAsync();
            var rect = Rectangle;
            ref var rRect = ref rect;

            if (rRect.Width is 0)
                rRect.Width = Window!.Rectangle.Width;
            if (rRect.Height is 0)
                rRect.Height = Window!.Rectangle.Height;
            Rectangle = controller.Bounds = rRect;
        }
        finally
        {
            PropertyChanged += EdgeWebView2_RectangleChanged;
        }
    }

    private async void EdgeWebView2_WindowChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(Window))
            return;

        var native = GetNative();
        await native.ResetControllerAsync();
        var controller = await native.GetControllerAsync();

        try
        {
            PropertyChanged -= EdgeWebView2_RectangleChanged;
            var rect = Rectangle;
            ref var rRect = ref rect;

            if (rRect.Width is 0)
                rRect.Width = Window!.Rectangle.Width;
            if (rRect.Height is 0)
                rRect.Height = Window!.Rectangle.Height;
            Rectangle = controller.Bounds = rRect;
        }
        finally
        {
            PropertyChanged += EdgeWebView2_RectangleChanged;
        }
    }

    public override async void NavigateTo([StringSyntax(StringSyntaxAttribute.Uri)] string uri)
    {
        var native = GetNative();
        var controller = await native.GetControllerAsync();
        controller.CoreWebView2.Navigate(uri);
    }

    private sealed class NativeWrapper(Win32Window window) : IDisposable
    {
        private bool _disposedValue;
        private static Task<CoreWebView2Environment>? s_environmentTask;
        private Task<CoreWebView2Controller>? _controllerTask;

        public static Task<CoreWebView2Environment> GetEnvironmentAsync()
        {
            s_environmentTask ??= CoreWebView2Environment
                .CreateAsync(
                    browserExecutableFolder: null,
                    userDataFolder: Path.Combine(AppContext.BaseDirectory, "WebView2Data"),
                    options: new());
            return s_environmentTask;
        }

        public async Task ResetControllerAsync()
        {
            if (_controllerTask is null)
                return;

            var controller = await _controllerTask;
            controller.Close();
            _controllerTask = null;
        }

        public async Task<CoreWebView2Controller> GetControllerAsync()
        {
            var environment = await GetEnvironmentAsync();

            _controllerTask ??= environment.CreateCoreWebView2ControllerAsync(window.HWND);
            return await _controllerTask;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _controllerTask?.Result.Close();
                }

                _disposedValue = true;
            }
        }

        // ~NativeWrapper()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
