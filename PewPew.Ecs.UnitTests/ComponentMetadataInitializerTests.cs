using System.Reflection;
using PewPew.Ecs.Core;
using PewPew.Ecs.Core.Internals;

namespace PewPew.Ecs.UnitTests;

public class ComponentMetadataInitializerTests
{
    [Fact]
    public void Test()
    {
        ComponentMetadataInitializer.FromAssembly(Assembly.GetExecutingAssembly());

        ComponentMetadata<DontUseThisComponent>.GlobalIndex.Should().NotBe(ComponentMetadata.InvalidIndex);
        ComponentMetadata<DontUseThisTag>.GlobalIndex.Should().NotBe(ComponentMetadata.InvalidIndex);
        ComponentMetadata<DontUseThisSingleton>.GlobalIndex.Should().NotBe(ComponentMetadata.InvalidIndex);
        ComponentMetadata<DontUseThisStaticBuffer>.GlobalIndex.Should().NotBe(ComponentMetadata.InvalidIndex);
    }

    private struct DontUseThisComponent : IComponent
    {
    }

    private struct DontUseThisTag : ITagComponent
    {
    }

    private struct DontUseThisSingleton : ISingletonComponent
    {
    }

    private struct DontUseThisStaticBuffer : IStaticBufferComponent
    {
    }
}