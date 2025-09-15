using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Shimakaze.UI.Native.WebView;

public abstract partial class WebView : ObservableObject
{
    [ObservableProperty]
    public partial Window? Window { get; set; }

    [ObservableProperty]
    public partial Rectangle Rectangle { get; set; }

    public abstract void NavigateTo([StringSyntax(StringSyntaxAttribute.Uri)] string uri);
}
