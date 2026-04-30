using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class BlittableBoolTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ImplicitFromBool_RoundTrip_PreservesValue(bool value)
    {
        BlittableBool blittable = value;
        bool result = blittable;

        result.Should().Be(value);
    }

    [Fact]
    public void ImplicitFromBool_True_IsTrue()
    {
        BlittableBool blittable = true;

        ((bool)blittable).Should().BeTrue();
    }

    [Fact]
    public void ImplicitFromBool_False_IsFalse()
    {
        BlittableBool blittable = false;

        ((bool)blittable).Should().BeFalse();
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(false, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    public void Equals_TwoInstances_ReturnsExpected(bool a, bool b, bool expected)
    {
        BlittableBool x = a;
        BlittableBool y = b;

        x.Equals(y).Should().Be(expected);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void GetHashCode_SameAsBool(bool value, bool sameAs)
    {
        BlittableBool blittable = value;

        blittable.GetHashCode().Should().Be(sameAs.GetHashCode());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToString_SameAsBool(bool value)
    {
        BlittableBool blittable = value;

        blittable.ToString().Should().Be(value.ToString());
    }

    [Fact]
    public void ComponentWithBlittableBool_InitAndAddAndGet_PreservesValue()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<BlittableBoolComponent>();

        EntityId entity = world.CreateEntityId();
        ref BlittableBoolComponent component = ref world.AddComponent<BlittableBoolComponent>(entity);
        component.Flag = true;
        component.OtherFlag = false;

        ref BlittableBoolComponent retrieved = ref world.GetComponent<BlittableBoolComponent>(entity);
        ((bool)retrieved.Flag).Should().BeTrue();
        ((bool)retrieved.OtherFlag).Should().BeFalse();
    }
}
