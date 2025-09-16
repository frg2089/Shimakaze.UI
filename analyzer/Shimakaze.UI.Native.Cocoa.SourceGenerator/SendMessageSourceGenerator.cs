using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Shimakaze.UI.Native.Cocoa.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class SendMessageSourceGenerator : IIncrementalGenerator
{
    public const string FullName = "Shimakaze.UI.Native.Cocoa.SendMessageAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            FullName,
            (node, ctx) => node is not null,
            (context, ctx) =>
            {
                if (context.Attributes.FirstOrDefault(i => i.AttributeClass?.ToDisplayString() is FullName) is not { } attribute)
                    return null;

                if (attribute.ConstructorArguments[0].Value is not string sel)
                    return null;

                if (context.TargetSymbol.ContainingType is not INamedTypeSymbol type)
                    return null;

                bool isStatic = context.TargetSymbol.IsStatic;
                return context.TargetSymbol switch
                {
                    IMethodSymbol method => new Metadata(
                        context.TargetSymbol,
                        false,
                        isStatic,
                        sel,
                        (method.ReturnType, method.GetReturnTypeAttributes(), "retVal"),
                        [.. method.Parameters.Select(p => (p.Type, p.GetAttributes(), p.Name))]),
                    IPropertySymbol property => new Metadata(
                        context.TargetSymbol,
                        true,
                        isStatic,
                        sel,
                        (property.Type, property.GetAttributes(), "retVal"),
                        []),
                    _ => null,
                };
            })
            .Where(i => i is not null)
            .Collect();

        context.RegisterSourceOutput(provider, (context, data) =>
        {
            foreach (var group in data.OfType<Metadata>().GroupBy(i => i.Symbol.ContainingType, SymbolEqualityComparer.Default))
            {
                context.AddSource(
                    $"{group.Key!.ToDisplayString()}.pinvoke.g.cs",
                    $$"""
                    namespace {{group.Key.ContainingNamespace.ToDisplayString()}};

                    {{group.Key.GetAccessibility()}} partial class {{group.Key.Name}}
                    {
                    {{string.Join("\r\n", group.Select(i => i.GetPInvokeCode()))}}
                    }
                    """
                    );
            }
        });
    }
}
internal sealed class Metadata(ISymbol symbol, bool isProperty, bool isStatic, string sel, (ITypeSymbol Type, ImmutableArray<AttributeData> Attributes, string Name) resultType, ImmutableArray<(ITypeSymbol Type, ImmutableArray<AttributeData> Attributes, string Name)> arguments)
{
    private readonly static Version Version = typeof(Metadata).Assembly.GetName().Version;
    public ISymbol Symbol { get; } = symbol;
    public bool IsProperty { get; } = isProperty;
    public bool IsStatic { get; } = isStatic;
    public (ITypeSymbol Type, ImmutableArray<AttributeData> Attributes, string Name) ResultType { get; } = resultType;
    public ImmutableArray<(ITypeSymbol Type, ImmutableArray<AttributeData> Attributes, string Name)> Arguments { get; } = arguments;
    public string SEL { get; } = sel;

    public string GetKey()
    {
        var arguments = Arguments.Select(Convert);
        return $"UnsafeCall(global::Shimakaze.UI.Native.Cocoa.Interop.ObjCSafeHandle self, global::Shimakaze.UI.Native.Cocoa.Interop.SEL op{string.Join(string.Empty, arguments.Select(static (arg, i) => $", {arg.ManagedTypeName} {arg.Name}"))});";
    }

    private static (string ManagedTypeName, string UnmanagedTypeName, string Name) Convert((ITypeSymbol Type, ImmutableArray<AttributeData> Attributes, string Name) info)
    {
        var managedTypeName = info.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (info.Type.NullableAnnotation is NullableAnnotation.Annotated)
            managedTypeName += '?';

        var unmanagedTypeName = managedTypeName;
        if (info.Type.IsSafeHandle())
            unmanagedTypeName = "nint";

        return (managedTypeName, unmanagedTypeName, info.Name);
    }

    public string GetPInvokeCode()
    {
        var resultType = Convert(ResultType);
        var arguments = Arguments.Select(Convert);

        using StringWriter writer = new();
        writer.WriteLine($"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Shimakaze.UI.Native.Cocoa.SourceGenerator\", \"{Version}\")]");
        writer.WriteLine($"[global::System.Runtime.CompilerServices.SkipLocalsInitAttribute]");
        writer.Write($"{Symbol.GetAccessibility()}{(IsStatic ? " static" : string.Empty)} partial {resultType.ManagedTypeName} {Symbol.Name}");
        if (!IsProperty)
            writer.Write($"({string.Join(", ", arguments.Select(static (arg, i) => $"{arg.ManagedTypeName} {arg.Name}"))})");

        writer.WriteLine();

        if (IsProperty)
            writer.Write("{ get");
        writer.WriteLine($$"""
        {
            {{resultType.NotVoid(() => resultType.ManagedDo(() => $"bool __invokeSucceeded = false;"))}}
            global::Shimakaze.UI.Native.Cocoa.Interop.ObjCSafeHandle self = {{(IsStatic ? $"ClassOf<{Symbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>()" : "this")}};
            global::Shimakaze.UI.Native.Cocoa.Interop.SEL op = "{{SEL}}";
            {{resultType.NotVoid(() => $"{resultType.ManagedTypeName} __retVal = default;")}}
            nint __self_native = default;
            nint __op_native = default;
            {{string.Join("\r\n    ", arguments.Select(static (arg, i) => arg.ManagedDo(() => $"{arg.UnmanagedTypeName} __{arg.Name}_native = default;")))}}
            {{resultType.NotVoid(() => resultType.ManagedDo(() => $"{resultType.UnmanagedTypeName} __retVal_native = default;"))}}

            {{resultType.NotVoid(() => resultType.ManagedDo(() => $"global::System.Runtime.InteropServices.Marshalling.SafeHandleMarshaller<{resultType.ManagedTypeName}>.ManagedToUnmanagedOut __retVal_native__marshaller = new();"))}}
            global::System.Runtime.InteropServices.Marshalling.SafeHandleMarshaller<global::Shimakaze.UI.Native.Cocoa.Interop.SEL>.ManagedToUnmanagedIn __op_native__marshaller = new();
            global::System.Runtime.InteropServices.Marshalling.SafeHandleMarshaller<global::Shimakaze.UI.Native.Cocoa.Interop.ObjCSafeHandle>.ManagedToUnmanagedIn __self_native__marshaller = new();
            {{string.Join("\r\n    ", arguments.Select(static (arg, i) => arg.ManagedDo(() => $"global::System.Runtime.InteropServices.Marshalling.SafeHandleMarshaller<{arg.ManagedTypeName}>.ManagedToUnmanagedIn __{arg.Name}_native__marshaller = new();")))}}

            try
            {
                __self_native__marshaller.FromManaged(self);
                __op_native__marshaller.FromManaged(op);
                {{string.Join("\r\n        ", arguments.Select(static (arg, i) => arg.ManagedDo(() => $"__{arg.Name}_native__marshaller.FromManaged({arg.Name});")))}}
                {
                    __self_native = __self_native__marshaller.ToUnmanaged();
                    __op_native = __op_native__marshaller.ToUnmanaged();
                    {{string.Join("\r\n            ", arguments.Select(static (arg, i) => arg.ManagedDo(() => $"__{arg.Name}_native = __{arg.Name}_native__marshaller.ToUnmanaged();")))}}
        
                    {{resultType.NotVoid(() => resultType.ManagedTypeName != resultType.UnmanagedTypeName
                        ? "__retVal_native = "
                        : "__retVal = ")}}__PInvoke(__self_native, __op_native{{string.Join(string.Empty, arguments.Select(static (arg, i) => arg.UnmanagedTypeName == arg.ManagedTypeName ? $", {arg.Name}" : $", __{arg.Name}_native"))}});
                }

        {{resultType.NotVoid(() => resultType.ManagedDo(() => $$$"""
                __invokeSucceeded = true;
                __retVal_native__marshaller.FromUnmanaged(__retVal_native);
                __retVal = __retVal_native__marshaller.ToManaged();
        """))}}
            }
            finally
            {
        {{resultType.NotVoid(() => resultType.ManagedDo(() => $$$"""
                if (__invokeSucceeded)
                {
                    __retVal_native__marshaller.Free();
                }
        """))}}
                __self_native__marshaller.Free();
                __op_native__marshaller.Free();
                {{string.Join("\r\n    ", arguments.Select(static (arg, i) => arg.ManagedDo(() => $"__{arg.Name}_native__marshaller.Free();")))}}
            }
        
            {{resultType.NotVoid(() => "return __retVal;")}}

            [global::System.Runtime.InteropServices.DllImportAttribute(global::Shimakaze.UI.Native.Cocoa.Native.LibraryNames.LibObjC, EntryPoint = "objc_msgSend", ExactSpelling = true)]
            static extern unsafe {{resultType.UnmanagedTypeName}} __PInvoke(nint __self_native, nint __op_native{{string.Join(string.Empty, arguments.Select(static (arg, i) => arg.UnmanagedTypeName == arg.ManagedTypeName ? $", {arg.ManagedTypeName} {arg.Name}" : $", {arg.UnmanagedTypeName} __{arg.Name}_native"))}});
        }
        """);
        if (IsProperty)
            writer.Write("}");
        return writer.ToString();
    }

}
