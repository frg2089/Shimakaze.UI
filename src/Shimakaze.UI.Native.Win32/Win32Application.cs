using System.Diagnostics;

using Microsoft.Win32.SafeHandles;

using Windows.Win32;

namespace Shimakaze.UI.Native.Win32;

public sealed class Win32Application(Dispatcher dispatcher) : Application(dispatcher)
{
    private bool _disposedValue;

    public readonly SafeProcessHandle HInstance = Process.GetCurrentProcess().SafeHandle;

    public static new Win32Application Instance => (Win32Application)Application.Instance;

    public override void Shutdown() => PInvoke.PostQuitMessage(0);

    internal new void OnInitialize() => base.OnInitialize();

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_disposedValue)
            return;

        if (disposing)
            HInstance.Dispose();

        _disposedValue = true;
    }
}