using Microsoft.CodeAnalysis;

namespace Shimakaze.UI.Native.Cocoa.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class SendMessageAttributeSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context => context.AddSource(
            "SendMessageAttribute.g.cs",
            """
            using System;
                                
            #nullable enable

            namespace Shimakaze.UI.Native.Cocoa;

            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
            internal sealed class SendMessageAttribute(string sel) : Attribute
            {
                public string SEL { get; } = sel;
            }
            """));
    }
}
