using Microsoft.CodeAnalysis;

namespace Shimakaze.UI.Native.Cocoa.SourceGenerator;

internal static class Extensions
{
    public static string NotVoid(this (string ManagedTypeName, string UnmanagedTypeName, string Name) info, Func<string> result)
    {
        if (info.ManagedTypeName is not "void" and not "System.Void")
            return result();

        return string.Empty;
    }
    public static string ManagedDo(this (string ManagedTypeName, string UnmanagedTypeName, string Name) info, Func<string> result)
    {
        if (info.ManagedTypeName != info.UnmanagedTypeName)
            return result();

        return string.Empty;
    }

    public static bool InheritsFrom(this ITypeSymbol derivedType, string baseType)
    {
        if (derivedType == null || baseType == null)
            return false;

        var current = derivedType;
        while (current != null)
        {
            if (current.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(baseType))
                return true;

            current = current.BaseType;
        }

        return false;
    }
    public static bool IsSafeHandle(this ITypeSymbol type) => type.InheritsFrom("global::System.Runtime.InteropServices.SafeHandle");

    public static string GetAccessibility(this ISymbol symbol, bool @public = true) => symbol.DeclaredAccessibility switch
    {
        Accessibility.Private => "private",
        Accessibility.ProtectedAndInternal => "private protected",
        Accessibility.Protected => "protected",
        Accessibility.Internal => "internal",
        Accessibility.ProtectedOrInternal => "protected internal",
        Accessibility.Public when @public => "public",
        _ => string.Empty,
    };
}