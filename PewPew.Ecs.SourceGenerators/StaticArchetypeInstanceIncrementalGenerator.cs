using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace PewPew.Ecs.SourceGenerators;

[Generator]
public sealed class StaticArchetypeInstanceIncrementalGenerator : IIncrementalGenerator
{
    private static readonly string StaticArchetypeInstanceAttribute = typeof(StaticArchetypeInstanceAttribute).FullName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m != null);

        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> compilationAndClasses
            = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2, spc));

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
            => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

        static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
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

                    if (fullName == StaticArchetypeInstanceAttribute)
                    {
                        // return the parent class of the method
                        return classDeclarationSyntax;
                    }
                }
            }

            return null;
        }
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        string archetypeContent = string.Empty;
        var archetypeFileStream = typeof(StaticArchetypeInstanceIncrementalGenerator).Assembly.GetManifestResourceStream("PewPew.Ecs.SourceGenerators.StaticArchetypeInstance.cs");
        using (StreamReader reader = new StreamReader(archetypeFileStream))
        {
            archetypeContent = reader.ReadToEnd();
        }

        IEnumerable<ClassDeclarationSyntax> distinctClasses = classes.Distinct();
        foreach (var classDeclarationSyntax in distinctClasses)
        {
            GenerateClass(context, classDeclarationSyntax, archetypeContent);
        }


        // context.ReportDiagnostic(Diagnostic.Create(
        //     DiagnosticDescriptors.FailedToParseMessage,
        //     structDeclarationSyntax.GetLocation(), // You can use GetLocation method on syntax nodes to pinpoint certain elements
        //     "Some text"));
        // context.AddSource("LoggerMessage.g.cs", SourceText.From(result, Encoding.UTF8));
    }

    private static void GenerateClass(SourceProductionContext context, ClassDeclarationSyntax classDeclarationSyntax, string archetypeContent)
    {
        if (classDeclarationSyntax.TypeParameterList == null)
            return;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
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
                var altGenericConstrainsReplace =
                    string.Join(Environment.NewLine, genericNames.Select(x => $"where T{x} : struct, IComponent"));
                var genericConstrainsReplace =
                    string.Join(Environment.NewLine, genericNames.Select(x => $"where {x} : struct, IComponent"));
                var componentFieldsReplace =
                    string.Join(Environment.NewLine, generics.Select(x => $"internal {x.Name}[] Components{x.Number};"));
                var componentArrayAllocationsReplace =
                    string.Join(Environment.NewLine, generics.Select(x => $"Components{x.Number} = new {x.Name}[maxComponentsPerType];"));
                var componentArrayCtorParamsReplace =
                    string.Join(Environment.NewLine, generics.Select(x => $"{x.Name}[] components{x.Number},"));
                var componentArrayInitsReplace =
                    string.Join(Environment.NewLine, generics.Select(x => $"Components{x.Number} = components{x.Number};"));

                var getComponentGenericCode =
                    string.Join(Environment.NewLine, generics.Select(
                        x =>
                            $$"""
                              
                              if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<{{x.Name}}>.GlobalIndex)
                              {
                                  return ref Unsafe.As<{{x.Name}}, T>(ref Components{{x.Number}}[index]);
                              }
                              """));

                var tryGetComponentGenericCode =
                    string.Join(Environment.NewLine, generics.Select(
                        x =>
                            $$"""

                              if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<{{x.Name}}>.GlobalIndex)
                              {
                                  ref var component = ref Unsafe.As<{{x.Name}}, T>(ref Components{{x.Number}}[denseIndex]);
                              
                                  componentRef = new ComponentRef<T>(ref component);
                              
                                  return true;
                              }
                              """));

                var componentRefReplace = $"ComponentRef<T1, T2{genericParametersReplace}>";

                var componentRefCtorParams =
                    ", " + string.Join(", ", generics.Select(x => $"ref Components{x.Number}[denseIndex]"));

                var addEntityParams =
                    ", " + string.Join(", ", generics.Select(x => $"{x.Name} component{x.Number}"));

                var addEntityCopyCode =
                    string.Join(Environment.NewLine, generics.Select(x => $"Components{x.Number}[denseIndex] = component{x.Number};"));

                var deleteEntityDefaultCode =
                    string.Join(Environment.NewLine, generics.Select(x => $"Components{x.Number}[count] = default;"));

                var deleteEntityReplaceCode =
                    string.Join(Environment.NewLine, generics.Select(x => $"Components{x.Number}[denseIndex] = Components{x.Number}[count];"));

                var getComponentsGenericCode =
                    string.Join(Environment.NewLine, generics.Select(
                        x =>
                            $$"""

                              if (ComponentMetadata<T>.GlobalIndex == ComponentMetadata<{{x.Name}}>.GlobalIndex)
                              {
                                  ref T[] components = ref Unsafe.As<{{x.Name}}[], T[]>(ref Components{{x.Number}});
                              
                                  return new Span<T>(components, 0, Count);
                              }
                              """));

                var ensureCapacityCode =
                    string.Join(Environment.NewLine, generics.Select(x => $"Array.Resize(ref Components{x.Number}, capacity);"));

                var getComponentMethod =
                    string.Join(Environment.NewLine, generics.Select(
                        x =>
                            $$"""

                              [MethodImpl(MethodImplOptions.AggressiveInlining)]
                              public ref {{x.Name}} GetComponent{{x.Number}}(EntityId entityId)
                              {
                                  DebugValidateEntityId(entityId);
                              
                                  return ref Components{{x.Number}}[GetRequiredDenseIndex(entityId)];
                              }
                              """));

                var moveEntityToCode =
                    string.Join(Environment.NewLine, generics.Select(x => $"var component{x.Number} = componentRef.Component{x.Number}; // copied"));

                var moveEntityToCode2 =
                    string.Join(Environment.NewLine, generics.Select(x => $"newTuple.Component{x.Number} = component{x.Number};"));

                var moveEntityToWorldCode =
                    string.Join(Environment.NewLine, generics.Select(x => $"World.AddComponent<{x.Name}>(entityId) = component{x.Number};"));

                var customMoveEntityToCodeT1 =
                    string.Join(Environment.NewLine, generics.Select(
                        x =>
                            $$"""

                              if (t1Index == ComponentMetadata<TT{{x.Number}}>.GlobalIndex)
                              {
                                  newTuple.Component{{x.Number}} = Unsafe.As<T1, TT{{x.Number}}>(ref component1);
                              }
                              """));

                var customMoveEntityToCodeT2 =
                    string.Join(Environment.NewLine, generics.Select(
                        x =>
                            $$"""

                              if (t2Index == ComponentMetadata<TT{{x.Number}}>.GlobalIndex)
                              {
                                  newTuple.Component{{x.Number}} = Unsafe.As<T2, TT{{x.Number}}>(ref component2);
                              }
                              """));

                var customMoveEntityToCodeTN =
                    string.Join(Environment.NewLine, generics.Select(x => GenerateCustomMoveEntityToCode(x, generics.Length + 2)));

                archetypeContent = archetypeContent.Replace("/*{GenericParameters}*/", genericParametersReplace);
                archetypeContent = archetypeContent.Replace("/*{AltGenericParameters}*/", altGenericParametersReplace);
                archetypeContent = archetypeContent.Replace("/*{GenericConstrains}*/", genericConstrainsReplace);
                archetypeContent = archetypeContent.Replace("/*{AltGenericConstrains}*/", altGenericConstrainsReplace);
                archetypeContent = archetypeContent.Replace("/*{ComponentFields}*/", componentFieldsReplace);
                archetypeContent = archetypeContent.Replace("/*{ComponentArrayAllocations}*/", componentArrayAllocationsReplace);
                archetypeContent = archetypeContent.Replace("/*{ComponentArrayCtorParams}*/", componentArrayCtorParamsReplace);
                archetypeContent = archetypeContent.Replace("/*{ComponentArrayInits}*/", componentArrayInitsReplace);
                archetypeContent = archetypeContent.Replace("/*{GetComponentGenericCode}*/", getComponentGenericCode);
                archetypeContent = archetypeContent.Replace("/*{TryGetComponentGenericCode}*/", tryGetComponentGenericCode);
                archetypeContent = archetypeContent.Replace("ComponentRef<T1, T2>", componentRefReplace);
                archetypeContent = archetypeContent.Replace("/*{ComponentRefCtorParams}*/", componentRefCtorParams);
                archetypeContent = archetypeContent.Replace("/*{AddEntityParams}*/", addEntityParams);
                archetypeContent = archetypeContent.Replace("/*{AddEntityCopyCode}*/", addEntityCopyCode);
                archetypeContent = archetypeContent.Replace("/*{DeleteEntityDefaultCode}*/", deleteEntityDefaultCode);
                archetypeContent = archetypeContent.Replace("/*{DeleteEntityReplaceCode}*/", deleteEntityReplaceCode);
                archetypeContent = archetypeContent.Replace("/*{GetComponentsGenericCode}*/", getComponentsGenericCode);
                archetypeContent = archetypeContent.Replace("/*{EnsureCapacityCode}*/", ensureCapacityCode);
                archetypeContent = archetypeContent.Replace("/*{GetComponentMethod}*/", getComponentMethod);
                archetypeContent = archetypeContent.Replace("/*{MoveEntityToCode}*/", moveEntityToCode);
                archetypeContent = archetypeContent.Replace("/*{MoveEntityToCode2}*/", moveEntityToCode2);
                archetypeContent = archetypeContent.Replace("/*{MoveEntityToWorldCode}*/", moveEntityToWorldCode);
                archetypeContent = archetypeContent.Replace("/*{CustomMoveEntityToCodeT1}*/", customMoveEntityToCodeT1);
                archetypeContent = archetypeContent.Replace("/*{CustomMoveEntityToCodeT2}*/", customMoveEntityToCodeT2);
                archetypeContent = archetypeContent.Replace("/*{CustomMoveEntityToCodeTN}*/", customMoveEntityToCodeTN);

                context.AddSource($"{classDeclarationSyntax.Identifier.Text}.{genericNames.Last()}.g.cs", SourceText.From(archetypeContent, Encoding.UTF8));
                // context.ReportDiagnostic(
                //     Diagnostic.Create(
                //         DiagnosticDescriptors.Test,
                //         classDeclarationSyntax.GetLocation(), // You can use GetLocation method on syntax nodes to pinpoint certain elements
                //         $"GenericNames: {genericNames[0]} {genericNames[1]}"));
            }
        }
    }

    private static string GenerateCustomMoveEntityToCode((string Name, string Number) generic, int count)
    {
        return $"var t{generic.Number}Index = ComponentMetadata<{generic.Name}>.GlobalIndex;" + Environment.NewLine + string.Join(Environment.NewLine, Enumerable.Range(1, count).Select(
            x =>
                $$"""

                  if (t{{generic.Number}}Index == ComponentMetadata<TT{{x}}>.GlobalIndex)
                  {
                      newTuple.Component{{x}} = Unsafe.As<{{generic.Name}}, TT{{x}}>(ref component{{generic.Number}});
                  }
                  """));
    }
}

internal static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor Test =
        new DiagnosticDescriptor(
            "SAMPLE001",
            "Test title",
            "{0}",
            "Parser",
            DiagnosticSeverity.Error,
            true);

    public static readonly DiagnosticDescriptor FailedToParseMessage = new("SAMPLE001",
        "Message parser failed",
        "Failed to parse message type '{0}'.", "Parser", DiagnosticSeverity.Error, true);
}

