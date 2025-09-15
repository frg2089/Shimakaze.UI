using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Shimakaze.UI.Native.Win32;

public sealed class Win32Window : Window
{
    private readonly string _className;

    internal HWND HWND { get; private set; }
    private bool _disposedValue;

    public unsafe Win32Window() : base()
    {
        _className = $"Shimakaze.UI.Win32Wrapper[{Process.GetCurrentProcess().ProcessName};{Parent?.Title ?? string.Empty};{Guid.NewGuid()}]";
        Debug.WriteLine($"Win32Window created: Class: {_className}");
        fixed (char* pClassName = _className)
        {
            WNDCLASSW wndClass = new()
            {
                lpfnWndProc = WndProc,
                hInstance = (HINSTANCE)Win32Application.Instance.HInstance.DangerousGetHandle(),
                lpszClassName = pClassName,
                style = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
            };
            PInvoke.RegisterClass(wndClass);

            HWND = PInvoke.CreateWindowEx(
                WINDOW_EX_STYLE.WS_EX_LEFT,
                wndClass.lpszClassName,
                null,
                WINDOW_STYLE.WS_OVERLAPPEDWINDOW,
                Rectangle.X is 0 ? PInvoke.CW_USEDEFAULT : Rectangle.X,
                Rectangle.Y is 0 ? PInvoke.CW_USEDEFAULT : Rectangle.Y,
                Rectangle.Width is 0 ? PInvoke.CW_USEDEFAULT : Rectangle.Width,
                Rectangle.Height is 0 ? PInvoke.CW_USEDEFAULT : Rectangle.Height,
                (Parent as Win32Window)?.HWND ?? HWND.Null,
                HMENU.Null,
                wndClass.hInstance,
                null
            );
            if (HWND.IsNull)
                throw new Win32Exception();

            if (!PInvoke.GetWindowRect(HWND, out var rect))
                throw new Win32Exception();

            Rectangle = rect;
            PropertyChanged += RectangleChanged;

            if (OperatingSystem.IsWindowsVersionAtLeast(6, 0, 6000))
            {
                BOOL value = true;
                PInvoke.DwmSetWindowAttribute(HWND, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, &value, (uint)Marshal.SizeOf(value));
            }
        }
    }

    private void RectangleChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(Rectangle))
            return;

        PInvoke.SetWindowPos(HWND, HWND.Null, Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height, SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
    }

    private LRESULT WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
            case PInvoke.WM_CREATE:
                break;
            case PInvoke.WM_PAINT:
                break;
            case PInvoke.WM_SHOWWINDOW:
                if (wParam.Value is not 0)
                    OnActivated();
                else
                    OnDeactivated();
                break;
            case PInvoke.WM_SIZE:
                if (!PInvoke.GetWindowRect(HWND, out var rect))
                    throw new Win32Exception();

                PropertyChanged -= RectangleChanged;
                Rectangle = rect;
                PropertyChanged += RectangleChanged;
                break;
            case PInvoke.WM_CLOSE:
                CancelEventArgs args = new();
                OnClosing(args);
                if (!args.Cancel)
                    PInvoke.DestroyWindow(HWND);
                break;
            case PInvoke.WM_DESTROY:
                OnClosed();
                break;
        }
        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    public override void Show()
    {
        PInvoke.ShowWindow(HWND, SHOW_WINDOW_CMD.SW_SHOWDEFAULT);
        PInvoke.UpdateWindow(HWND);
    }

    public override void Close() => PInvoke.CloseWindow(HWND);

    protected override void Dispose(bool disposing)
    {
        base.Dispose();
        if (_disposedValue)
            return;
        
        if (disposing)
        {
        }

        PInvoke.DestroyWindow(HWND);
        PInvoke.UnregisterClass(_className, Win32Application.Instance.HInstance);
        _disposedValue = true;
    }

}