using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace PewPew.Ecs.SourceGenerators;

[Generator]
public sealed class EntityIdIncrementalGenerator : IIncrementalGenerator
{
    private static readonly string EntityIdAttribute = typeof(EntityIdAttribute).FullName;
    private static readonly string StaticArchetypeInstanceAttribute = typeof(StaticArchetypeInstanceAttribute).FullName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<StructDeclarationSyntax> structDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m != null);

        IncrementalValueProvider<(Compilation, ImmutableArray<StructDeclarationSyntax>)> compilationAndStructs
            = context.CompilationProvider.Combine(structDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndStructs, static (spc, source) => Execute(source.Item1, source.Item2, spc));

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is StructDeclarationSyntax m && m.AttributeLists.Count > 0;

        static StructDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var structDeclarationSyntax = (StructDeclarationSyntax)context.Node;

            foreach (AttributeListSyntax attributeListSyntax in structDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    IMethodSymbol attributeSymbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;
                    if (attributeSymbol == null)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    if (fullName == EntityIdAttribute)
                    {
                        // return the parent class of the method
                        return structDeclarationSyntax;
                    }
                }
            }

            return null;
        }
    }

    private static void Execute(Compilation compilation, ImmutableArray<StructDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        string entityIdContent = string.Empty;
        var entityIdFileStream = typeof(EntityIdIncrementalGenerator).Assembly.GetManifestResourceStream("PewPew.Ecs.SourceGenerators.EntityId.cs");
        using (StreamReader reader = new StreamReader(entityIdFileStream))
        {
            entityIdContent = reader.ReadToEnd();
        }

        IEnumerable<StructDeclarationSyntax> distinctStructs = classes.Distinct();
        foreach (var structDeclarationSyntax in distinctStructs)
        {
            // todo: adjust namespace
            var source = entityIdContent.Replace("EntityId", structDeclarationSyntax.Identifier.Text);

            context.AddSource($"{structDeclarationSyntax.Identifier.Text}.g.cs", SourceText.From(source, Encoding.UTF8));
        }


        // context.ReportDiagnostic(Diagnostic.Create(
        //     DiagnosticDescriptors.FailedToParseMessage,
        //     structDeclarationSyntax.GetLocation(), // You can use GetLocation method on syntax nodes to pinpoint certain elements
        //     "Some text"));
        // context.AddSource("LoggerMessage.g.cs", SourceText.From(result, Encoding.UTF8));
    }

    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor FailedToParseMessage = new("SAMPLE001",
            "Message parser failed",
            "Failed to parse message type '{0}'.", "Parser", DiagnosticSeverity.Error, true);
    }
}

