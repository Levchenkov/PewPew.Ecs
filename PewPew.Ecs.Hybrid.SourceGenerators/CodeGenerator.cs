using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PewPew.Ecs.Hybrid.SourceGenerators;

// todo: use different way to generate code. This variant is unreadable
public class CodeGenerator
{
    public string GenerateAllCode(ArchetypeInfo info, IReadOnlyList<ArchetypeInfo> allInfos)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine("using PewPew.Ecs.Core;");
        sb.AppendLine("using PewPew.Ecs.Filters.Masks;");
        sb.AppendLine("using PewPew.Ecs.Hybrid;");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(info.Namespace))
        {
            sb.AppendLine($"namespace {info.Namespace};");
            sb.AppendLine();
        }

        sb.Append(GenerateArchetypeRefStruct(info, allInfos));
        sb.AppendLine();
        sb.Append(GenerateExtensionsClass(info));

        return sb.ToString();
    }

    private string GenerateArchetypeRefStruct(ArchetypeInfo info, IReadOnlyList<ArchetypeInfo> allInfos)
    {
        var sb = new StringBuilder();
        var componentsGenericArgs = string.Join(", ", info.Fields.Select(f => f.TypeName));

        sb.AppendLine($"public readonly ref struct {info.ArchetypeName}");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly StaticArchetype<{info.MaskType}, {componentsGenericArgs}> _archetype;");
        sb.AppendLine();
        sb.AppendLine($"    public {info.ArchetypeName}(StaticArchetype<{info.MaskType}, {componentsGenericArgs}> archetype)");
        sb.AppendLine("    {");
        sb.AppendLine("        _archetype = archetype;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Get method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public {info.ValueTypeName} Get(EntityId entityId)");
        sb.AppendLine("    {");
        sb.AppendLine("        var index = _archetype.GetRequiredIndexUnsafe(entityId);");
        sb.AppendLine();
        var refParams = string.Join(",\n            ",
            info.Fields.Select(f => $"ref _archetype.{f.GetComponentN}Unsafe(index)"));
        sb.AppendLine($"        return new {info.ValueTypeName}(");
        sb.AppendLine($"            {refParams});");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Getter methods for each component
        foreach (var field in info.Fields)
        {
            sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine($"    public ref {field.TypeName} Get{field.FieldName}(EntityId entityId) => ref _archetype.{field.GetComponentN}(entityId);");
            sb.AppendLine();
        }

        // Add(entityId, value) method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public void Add(EntityId entityId, {info.ValueTypeName} value)");
        sb.AppendLine("    {");
        var addParams = string.Join(", ", info.Fields.Select(f => $"value.{f.FieldName}"));
        sb.AppendLine($"        _archetype.Add(entityId, {addParams});");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Add(entityId) method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public {info.ValueTypeName} Add(EntityId entityId)");
        sb.AppendLine("    {");
        sb.AppendLine("        var result = _archetype.Add(entityId);");
        sb.AppendLine();
        var returnParams = string.Join(",\n            ",
            info.Fields.Select(f => $"ref result.{f.ComponentN}"));
        sb.AppendLine($"        return new {info.ValueTypeName}(");
        sb.AppendLine($"            {returnParams});");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Count property
        sb.AppendLine("    public int Count");
        sb.AppendLine("    {");
        sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("        get => _archetype.Count;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Entities property
        sb.AppendLine("    public Span<EntityId> Entities");
        sb.AppendLine("    {");
        sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("        get => _archetype.Entities;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Has method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("    public bool Has(EntityId entityId) => _archetype.Has(entityId);");
        sb.AppendLine();

        // TryGetComponents method
        var componentRefType = $"ComponentRef<{componentsGenericArgs}>";
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public bool TryGetComponents(EntityId entityId, out {componentRefType} componentRef)");
        sb.AppendLine("        => _archetype.TryGetComponents(entityId, out componentRef);");
        sb.AppendLine();

        // Create method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("    public void Create(EntityId entityId) => _archetype.Create(entityId);");
        sb.AppendLine();

        // Delete method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("    public void Delete(EntityId entityId) => _archetype.Delete(entityId);");
        sb.AppendLine();

        // MoveEntityTo method — underlying StaticArchetype.MoveEntityTo hardcodes BitMask64 as the target mask
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public void MoveEntityTo(EntityId entityId, StaticArchetype<BitMask64, {componentsGenericArgs}> target)");
        sb.AppendLine("        => _archetype.MoveEntityTo(entityId, target);");
        sb.AppendLine();

        // Cross-archetype MoveEntityTo overloads — all other BitMask64 archetypes in the compilation
        foreach (var target in allInfos.Where(t => t != info && t.MaskType == "BitMask64"))
        {
            var targetFullName = string.IsNullOrEmpty(target.Namespace)
                ? target.ArchetypeName
                : $"global::{target.Namespace}.{target.ArchetypeName}";
            sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine($"    public void MoveEntityTo(EntityId entityId, {targetFullName} target)");
            sb.AppendLine($"        => _archetype.MoveEntityTo(entityId, target.Archetype);");
            sb.AppendLine();
        }

        // Archetype property — exposes inner StaticArchetype for interop with MoveEntityTo across archetypes
        sb.AppendLine($"    internal StaticArchetype<{info.MaskType}, {componentsGenericArgs}> Archetype");
        sb.AppendLine("    {");
        sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("        get => _archetype;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // MoveEntityToWorld method
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("    public void MoveEntityToWorld(EntityId entityId) => _archetype.MoveEntityToWorld(entityId);");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private string GenerateExtensionsClass(ArchetypeInfo info)
    {
        var sb = new StringBuilder();
        var componentsGenericArgs = string.Join(", ", info.Fields.Select(f => f.TypeName));
        var initMethodName = $"Init{info.ArchetypeName}";
        var getMethodName = $"Get{info.ArchetypeName}";
        var worldType = $"HybridWorld<{info.MaskType}>";

        sb.AppendLine($"public static class {info.ClassName}HybridWorldExtensions");
        sb.AppendLine("{");
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public static void {initMethodName}(this {worldType} world)");
        sb.AppendLine("    {");
        sb.AppendLine($"        world.InitStaticArchetype<{componentsGenericArgs}>();");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"    public static {info.ArchetypeName} {getMethodName}(this {worldType} world)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var archetypeRef = world.GetStaticArchetype<{componentsGenericArgs}>();");
        sb.AppendLine($"        return new {info.ArchetypeName}(archetypeRef);");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

}

