using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PewPew.Ecs.Hybrid.SourceGenerators;

public static class ArchetypeAnalyzer
{
    public static ArchetypeInfo? TryAnalyze(StructDeclarationSyntax structDecl, SemanticModel semanticModel)
    {
        try
        {
            var structSymbol = semanticModel.GetDeclaredSymbol(structDecl);
            if (structSymbol is null)
                return null;

            // Extract namespace
            var ns = structSymbol.ContainingNamespace.IsGlobalNamespace
                ? ""
                : structSymbol.ContainingNamespace.ToDisplayString();

            // Extract [Archetype] attribute by full name (reliable across assemblies)
            var archetypeAttr = structSymbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "PewPew.Ecs.Hybrid.ArchetypeAttribute");

            if (archetypeAttr is null)
                return null;

            // Parse attribute parameters
            var maskType = "BitMask64"; // default
            var customArchetypeName = "";

            for (int i = 0; i < archetypeAttr.NamedArguments.Length; i++)
            {
                var name = archetypeAttr.NamedArguments[i].Key;
                var value = archetypeAttr.NamedArguments[i].Value;

                if (name == "Mask" && value.Value is INamedTypeSymbol maskSymbol)
                {
                    maskType = maskSymbol.Name;
                }
                else if (name == "ArchetypeName" && value.Value is string archName)
                {
                    customArchetypeName = archName;
                }
            }

            // Extract public ref fields
            var fields = ExtractComponentFields(structDecl, semanticModel);
            if (fields.Count < 2)
                return null; // Require at least 2 components

            var archetypeName = !string.IsNullOrEmpty(customArchetypeName)
                ? customArchetypeName
                : $"{structSymbol.Name}Archetype";

            return new ArchetypeInfo
            {
                ClassName = structSymbol.Name,
                Namespace = ns,
                MaskType = maskType,
                Fields = fields,
                ArchetypeName = archetypeName,
                ValueTypeName = structSymbol.Name
            };
        }
        catch
        {
            return null;
        }
    }

    private static List<ComponentField> ExtractComponentFields(StructDeclarationSyntax structDecl, SemanticModel semanticModel)
    {
        var fields = new List<ComponentField>();
        var index = 0;

        foreach (var member in structDecl.Members.OfType<FieldDeclarationSyntax>())
        {
            // Must be public
            var isPublic = false;
            foreach (var modifier in member.Modifiers)
            {
                if (modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
                {
                    isPublic = true;
                    break;
                }
            }

            if (!isPublic)
                continue;

            // In C# 11, ref fields use RefTypeSyntax as the field type, e.g.:
            //   public ref Position Position;
            //   └─ RefTypeSyntax("ref Position")
            // Rather than a ref modifier on the field declaration.
            if (member.Declaration.Type is not Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax refTypeSyntax)
                continue;

            var typeSymbol = semanticModel.GetTypeInfo(refTypeSyntax.Type).Type;
            var innerTypeName = typeSymbol is not null
                ? typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                : refTypeSyntax.Type.ToString();

            foreach (var variable in member.Declaration.Variables)
            {
                fields.Add(new ComponentField
                {
                    TypeName = innerTypeName,
                    FieldName = variable.Identifier.Text,
                    Index = index++
                });
            }
        }

        return fields;
    }
}


