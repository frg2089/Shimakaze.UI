namespace Shimakaze.UI.Native.Cocoa.Interop;

/// <summary>
/// <see cref="https://learn.microsoft.com/dotnet/api/appkit.nsapplicationactivationpolicy"/>
/// </summary>
internal enum NSApplicationActivationPolicy
{
    Regular = 0,
    Accessory = 1,
    Prohibited = 2,
}