using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class BlittableCharTests
{
    [Theory]
    [InlineData('A')]
    [InlineData('z')]
    [InlineData('0')]
    [InlineData(' ')]
    [InlineData('\0')]
    [InlineData('\uFFFF')]
    public void ImplicitFromChar_RoundTrip_PreservesValue(char value)
    {
        BlittableChar blittable = value;
        char result = blittable;

        result.Should().Be(value);
    }

    [Theory]
    [InlineData('A', 'A', true)]
    [InlineData('A', 'B', false)]
    [InlineData('\0', '\0', true)]
    [InlineData('\uFFFF', '\uFFFF', true)]
    [InlineData('\uFFFF', '\0', false)]
    public void Equals_TwoInstances_ReturnsExpected(char a, char b, bool expected)
    {
        BlittableChar x = a;
        BlittableChar y = b;

        x.Equals(y).Should().Be(expected);
    }

    [Theory]
    [InlineData('A')]
    [InlineData('\0')]
    [InlineData('\uFFFF')]
    public void GetHashCode_SameAsChar(char value)
    {
        BlittableChar blittable = value;

        blittable.GetHashCode().Should().Be(value.GetHashCode());
    }

    [Theory]
    [InlineData('A')]
    [InlineData('z')]
    [InlineData('\0')]
    public void ToString_SameAsChar(char value)
    {
        BlittableChar blittable = value;

        blittable.ToString().Should().Be(value.ToString());
    }

    [Fact]
    public void ComponentWithBlittableChar_InitAndAddAndGet_PreservesValue()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<BlittableCharComponent>();

        EntityId entity = world.CreateEntityId();
        ref BlittableCharComponent component = ref world.AddComponent<BlittableCharComponent>(entity);
        component.Letter = 'X';
        component.Symbol = '\uFFFF';

        ref BlittableCharComponent retrieved = ref world.GetComponent<BlittableCharComponent>(entity);
        ((char)retrieved.Letter).Should().Be('X');
        ((char)retrieved.Symbol).Should().Be('\uFFFF');
    }
}
