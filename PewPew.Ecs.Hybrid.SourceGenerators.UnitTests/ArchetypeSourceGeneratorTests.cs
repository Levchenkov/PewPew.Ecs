using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;
using PewPew.Ecs.Hybrid;
using PewPew.Ecs.Hybrid.SourceGenerators;

namespace PewPew.Ecs.Hybrid.SourceGenerators.UnitTests;

public class ArchetypeSourceGeneratorTests
{
    [Fact]
    public void Generates_wrapper_and_extensions_for_valid_archetype()
    {
        const string source = """
namespace Demo;

using PewPew.Ecs.Core;

public struct Position : IComponent { }
public struct Speed : IComponent { }

[PewPew.Ecs.Hybrid.Archetype]
public ref struct Player
{
    public ref Position Position;
    public ref Speed Speed;

    public Player(ref Position position, ref Speed speed)
    {
        Position = ref position;
        Speed = ref speed;
    }
}
""";

        var result = RunGenerator(source, out _);

        result.GeneratedSources.Should().Contain(x => x.HintName == "PewPew.Ecs.Hybrid.ArchetypeAttribute.g.cs");
        result.GeneratedSources.Should().Contain(x => x.HintName == "Player.Archetype.g.cs");

        var generatedCode = GetGeneratedCode(result, "Player.Archetype.g.cs");
        generatedCode.Should().Contain("public readonly ref struct PlayerArchetype");
        generatedCode.Should().Contain("public static class PlayerHybridWorldExtensions");
        generatedCode.Should().Contain("public ref Position GetPosition(EntityId entityId)");
        generatedCode.Should().Contain("public ref Speed GetSpeed(EntityId entityId)");
        generatedCode.Should().Contain("public static void InitPlayerArchetype(this HybridWorld<BitMask64> world)");
        generatedCode.Should().Contain("public static PlayerArchetype GetPlayerArchetype(this HybridWorld<BitMask64> world)");
        generatedCode.Should().Contain("world.InitStaticArchetype<Position, Speed>();");
        generatedCode.Should().Contain("var archetypeRef = world.GetStaticArchetype<Position, Speed>();");
    }

    [Fact]
    public void Respects_custom_mask_and_archetype_name()
    {
        const string source = """
namespace Demo;

using PewPew.Ecs.Core;
using PewPew.Ecs.Filters.Masks;

public struct Position : IComponent { }
public struct Speed : IComponent { }

[PewPew.Ecs.Hybrid.Archetype(ArchetypeName = "HeroPack", Mask = typeof(BitMask128))]
public ref struct Hero
{
    public ref Position Position;
    public ref Speed Speed;

    public Hero(ref Position position, ref Speed speed)
    {
        Position = ref position;
        Speed = ref speed;
    }
}
""";

        var result = RunGenerator(source, out _);

        var generatedCode = GetGeneratedCode(result, "Hero.Archetype.g.cs");
        generatedCode.Should().Contain("public readonly ref struct HeroPack");
        generatedCode.Should().Contain("private readonly StaticArchetype<BitMask128, Position, Speed> _archetype;");
        generatedCode.Should().Contain("public static void InitHeroPack(this HybridWorld<BitMask128> world)");
        generatedCode.Should().Contain("public static HeroPack GetHeroPack(this HybridWorld<BitMask128> world)");
    }

    [Fact]
    public void Does_not_generate_wrapper_when_less_than_two_public_ref_fields_exist()
    {
        const string source = """
namespace Demo;

using PewPew.Ecs.Core;

public struct Position : IComponent { }
public struct Speed : IComponent { }

[PewPew.Ecs.Hybrid.Archetype]
public ref struct Broken
{
    public ref Position Position;
    private ref Speed Speed;

    public Broken(ref Position position, ref Speed speed)
    {
        Position = ref position;
        Speed = ref speed;
    }
}
""";

        var result = RunGenerator(source, out _);

        result.GeneratedSources.Should().ContainSingle(x => x.HintName == "PewPew.Ecs.Hybrid.ArchetypeAttribute.g.cs");
        result.GeneratedSources.Should().NotContain(x => x.HintName == "Broken.Archetype.g.cs");
    }

    private static GeneratorRunResult RunGenerator(string source, out Compilation outputCompilation)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions, path: "Input.cs");

        var compilation = CSharpCompilation.Create(
            assemblyName: "GeneratorTests",
            syntaxTrees: new[] { syntaxTree },
            references: GetMetadataReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: new[] { new ArchetypeSourceGenerator().AsSourceGenerator() },
            parseOptions: parseOptions);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out outputCompilation, out var generatorDiagnostics);

        generatorDiagnostics.Should().BeEmpty();
        outputCompilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        return driver.GetRunResult().Results.Should().ContainSingle().Subject;
    }

    private static ImmutableArray<MetadataReference> GetMetadataReferences()
    {
        var trustedPlatformAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));

        var projectAssemblies = new[]
        {
            typeof(object).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(EntityId).Assembly.Location,
            typeof(BitMask64).Assembly.Location,
            typeof(HybridWorld).Assembly.Location,
            typeof(ArchetypeSourceGenerator).Assembly.Location,
        }
            .Distinct()
            .Select(path => MetadataReference.CreateFromFile(path));

        return trustedPlatformAssemblies
            .Concat(projectAssemblies)
            .OfType<PortableExecutableReference>()
            .GroupBy(reference => reference.FilePath, StringComparer.OrdinalIgnoreCase)
            .Select(group => (MetadataReference)group.First())
            .ToImmutableArray();
    }

    private static string GetGeneratedCode(GeneratorRunResult result, string hintName)
    {
        return result.GeneratedSources.Single(source => source.HintName == hintName).SourceText.ToString();
    }
}


