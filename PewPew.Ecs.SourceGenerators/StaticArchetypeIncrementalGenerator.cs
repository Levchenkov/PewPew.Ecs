using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace PewPew.Ecs.SourceGenerators;

[Generator]
public sealed class StaticArchetypeIncrementalGenerator : IIncrementalGenerator
{
    private static readonly string StaticArchetypeAttribute = typeof(StaticArchetypeAttribute).FullName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<StructDeclarationSyntax> structDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m != null);

        IncrementalValueProvider<(Compilation Left, ImmutableArray<StructDeclarationSyntax> Right)> compilationAndClasses
            = context.CompilationProvider.Combine(structDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2, spc));

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

                    if (fullName == StaticArchetypeAttribute)
                    {
                        // return the parent class of the method
                        return structDeclarationSyntax;
                    }
                }
            }

            return null;
        }
    }

    private static void Execute(Compilation compilation, ImmutableArray<StructDeclarationSyntax> structs, SourceProductionContext context)
    {
        if (structs.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        string archetypeContent = string.Empty;
        var archetypeFileStream = typeof(StaticArchetypeIncrementalGenerator).Assembly.GetManifestResourceStream("PewPew.Ecs.SourceGenerators.StaticArchetype.cs");
        using (StreamReader reader = new StreamReader(archetypeFileStream))
        {
            archetypeContent = reader.ReadToEnd();
        }

        IEnumerable<StructDeclarationSyntax> distinctStructs = structs.Distinct();
        foreach (var structDeclarationSyntax in distinctStructs)
        {
            GenerateClass(context, structDeclarationSyntax, archetypeContent);
        }
    }

    private static void GenerateClass(SourceProductionContext context, StructDeclarationSyntax structDeclarationSyntax, string archetypeContent)
    {
        if (structDeclarationSyntax.TypeParameterList == null)
            return;

        foreach (var attributeListSyntax in structDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (attributeSyntax.ArgumentList == null)
                    return;

                var genericNames = attributeSyntax.ArgumentList.Arguments.Select(x => x.ToString().Trim('"')).ToArray();
                var genericNumbers= genericNames.Select(x => x.Replace("T", "")).ToArray();
                var generics = genericNames.Zip(genericNumbers, (a, b) => (Name: a, Number: b)).ToArray();

                var genericParametersReplace = $", {string.Join(", ", genericNames)}";
                var altGenericParametersReplace =
                    ", " + string.Join(", ", generics.Select(x => $"T{x.Name}"));
                var genericConstrainsReplace =
                    string.Join(Environment.NewLine, genericNames.Select(x => $"where {x} : struct, IComponent"));
                var altGenericConstrainsReplace =
                    string.Join(Environment.NewLine, genericNames.Select(x => $"where T{x} : struct, IComponent"));
                 var componentFieldsReplace =
                     string.Join(Environment.NewLine, generics.Select(x => $"private readonly Ref<{x.Name}> _component{x.Number};"));
                 var componentFieldsInit =
                     string.Join(Environment.NewLine, generics.Select(x => $"_component{x.Number} = new Ref<{x.Name}>(ref instance.Components{x.Number}[0]);"));

                 var componentRefReplace = $"ComponentRef<T1, T2{genericParametersReplace}>";

                 var componentProperties =
                     string.Join(Environment.NewLine, generics.Select(
                         x =>
                             $$"""

                               public Span<{{x.Name}}> Components{{x.Number}}
                               {
                                   [MethodImpl(MethodImplOptions.AggressiveInlining)]
                                   get => new(_instance.Components{{x.Number}}, 0, Count);
                               }
                               """));

                 var getComponentMethods =
                     string.Join(Environment.NewLine, generics.Select(
                         x =>
                             $$"""

                               [MethodImpl(MethodImplOptions.AggressiveInlining)]
                               public ref {{x.Name}} GetComponent{{x.Number}}(EntityId entityId) => ref _instance.GetComponent{{x.Number}}(entityId);
                               """));

                 var getComponentUnsafeMethods =
                     string.Join(Environment.NewLine, generics.Select(
                         x =>
                             $$"""

                               [MethodImpl(MethodImplOptions.AggressiveInlining)]
                               public ref {{x.Name}} GetComponent{{x.Number}}Unsafe(int index)
                               {
                                   DebugValidateIndex(index);
                               
                                   return ref Unsafe.Add(ref _component{{x.Number}}.Value, index);
                               }
                               """));

                 var addMethodParams =
                     ", " + string.Join(", ", generics.Select(x => $"{x.Name} component{x.Number}"));

                 var callParams =
                     ", " + string.Join(", ", generics.Select(x => $"component{x.Number}"));

                archetypeContent = archetypeContent.Replace("/*{GenericParameters}*/", genericParametersReplace);
                archetypeContent = archetypeContent.Replace("/*{AltGenericParameters}*/", altGenericParametersReplace);
                archetypeContent = archetypeContent.Replace("/*{GenericConstrains}*/", genericConstrainsReplace);
                archetypeContent = archetypeContent.Replace("/*{AltGenericConstrains}*/", altGenericConstrainsReplace);
                archetypeContent = archetypeContent.Replace("/*{ComponentFields}*/", componentFieldsReplace);
                archetypeContent = archetypeContent.Replace("/*{ComponentFieldsInit}*/", componentFieldsInit);
                archetypeContent = archetypeContent.Replace("ComponentRef<T1, T2>", componentRefReplace);
                archetypeContent = archetypeContent.Replace("/*{ComponentProperties}*/", componentProperties);
                archetypeContent = archetypeContent.Replace("/*{GetComponentMethods}*/", getComponentMethods);
                archetypeContent = archetypeContent.Replace("/*{GetComponentUnsafeMethods}*/", getComponentUnsafeMethods);
                archetypeContent = archetypeContent.Replace("/*{AddMethodParams}*/", addMethodParams);
                archetypeContent = archetypeContent.Replace("/*{CallParams}*/", callParams);

                context.AddSource($"{structDeclarationSyntax.Identifier.Text}.{genericNames.Last()}.g.cs", SourceText.From(archetypeContent, Encoding.UTF8));
                // context.ReportDiagnostic(
                //     Diagnostic.Create(
                //         DiagnosticDescriptors.Test,
                //         classDeclarationSyntax.GetLocation(), // You can use GetLocation method on syntax nodes to pinpoint certain elements
                //         $"GenericNames: {genericNames[0]} {genericNames[1]}"));
            }
        }
    }
}

