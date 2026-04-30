using System.Text;
using PewPew.Ecs.Core;

namespace PewPew.Ecs.UnitTests;

public class BlittableString32Tests
{
    // -- AsSpan / Set --

    [Fact]
    public void Default_IsEmpty()
    {
        BlittableString32 s = default;

        s.IsEmpty.Should().BeTrue();
        s.Length.Should().Be(0);
        s.AsReadOnlySpan().IsEmpty.Should().BeTrue();
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("world")]
    [InlineData("")]
    [InlineData("abc123!@#")]
    public void Set_ThenAsSpan_RoundTrip(string input)
    {
        BlittableString32 s = default;
        var utf8 = Encoding.UTF8.GetBytes(input);

        s.Set(utf8);

        s.AsReadOnlySpan().SequenceEqual(utf8).Should().BeTrue();
        s.Length.Should().Be(utf8.Length);
    }

    [Fact]
    public void Set_InputLongerThanCapacity_TruncatesToCapacity()
    {
        BlittableString32 s = default;
        var utf8 = new byte[BlittableString32.Capacity + 10];
        for (int i = 0; i < utf8.Length; i++) utf8[i] = (byte)('a' + i % 26);

        var written = s.Set(utf8);

        written.Should().Be(BlittableString32.Capacity);
        s.Length.Should().Be(BlittableString32.Capacity);
    }

    [Fact]
    public void Set_ExactCapacity_StoresAll()
    {
        BlittableString32 s = default;
        var utf8 = new byte[BlittableString32.Capacity];
        for (int i = 0; i < utf8.Length; i++) utf8[i] = (byte)('A' + i % 26);

        var written = s.Set(utf8);

        written.Should().Be(BlittableString32.Capacity);
        s.AsReadOnlySpan().SequenceEqual(utf8).Should().BeTrue();
    }

    // -- TrySet --

    [Fact]
    public void TrySet_FitsInCapacity_ReturnsTrueAndStores()
    {
        BlittableString32 s = default;
        var utf8 = "fit"u8.ToArray();

        var result = s.TrySet(utf8);

        result.Should().BeTrue();
        s.AsReadOnlySpan().SequenceEqual(utf8).Should().BeTrue();
    }

    [Fact]
    public void TrySet_ExceedsCapacity_ReturnsFalseAndDoesNotModify()
    {
        BlittableString32 s = default;
        var initial = "initial"u8.ToArray();
        s.Set(initial);

        var tooLong = new byte[BlittableString32.Capacity + 1];
        var result = s.TrySet(tooLong);

        result.Should().BeFalse();
        s.AsReadOnlySpan().SequenceEqual(initial).Should().BeTrue();
    }

    // -- TryCreate --

    [Theory]
    [InlineData("hello")]
    [InlineData("PewPew.Ecs")]
    [InlineData("")]
    [InlineData("32chars_fit_exactly_in_32bytes!!")]
    public void TryCreate_FitsInCapacity_ReturnsTrueAndStoresUtf8(string input)
    {
        var success = BlittableString32.TryCreate(input, out var s);

        success.Should().BeTrue();
        var expected = Encoding.UTF8.GetBytes(input);
        s.AsReadOnlySpan().SequenceEqual(expected).Should().BeTrue();
    }

    [Fact]
    public void TryCreate_StringTooLong_ReturnsFalse()
    {
        var tooLong = new string('x', BlittableString32.Capacity + 1);

        var success = BlittableString32.TryCreate(tooLong, out var s);

        success.Should().BeFalse();
        s.IsEmpty.Should().BeTrue();
    }

    // -- ToString --

    [Theory]
    [InlineData("hello")]
    [InlineData("PewPew")]
    [InlineData("")]
    public void ToString_AfterTryCreate_ReturnsOriginalString(string input)
    {
        BlittableString32.TryCreate(input, out var s);

        s.ToString().Should().Be(input);
    }

    // -- Equality --

    [Fact]
    public void Equals_SameContent_ReturnsTrue()
    {
        BlittableString32.TryCreate("test", out var a);
        BlittableString32.TryCreate("test", out var b);

        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentContent_ReturnsFalse()
    {
        BlittableString32.TryCreate("foo", out var a);
        BlittableString32.TryCreate("bar", out var b);

        a.Equals(b).Should().BeFalse();
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Equals_DefaultVsEmpty_ReturnsTrue()
    {
        BlittableString32 a = default;
        BlittableString32.TryCreate("", out var b);

        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameContent_ReturnsSameHash()
    {
        BlittableString32.TryCreate("hashme", out var a);
        BlittableString32.TryCreate("hashme", out var b);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentContent_ReturnsDifferentHash()
    {
        BlittableString32.TryCreate("foo", out var a);
        BlittableString32.TryCreate("bar", out var b);

        a.GetHashCode().Should().NotBe(b.GetHashCode());
    }

    // -- ECS integration --

    [Fact]
    public void ComponentWithInlineString_AddAndGet_PreservesValue()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<BlittableString32Component>();

        var entity = world.CreateEntityId();
        ref var component = ref world.AddComponent<BlittableString32Component>(entity);
        BlittableString32.TryCreate("player_one", out component.Name);

        ref var retrieved = ref world.GetComponent<BlittableString32Component>(entity);
        retrieved.Name.ToString().Should().Be("player_one");
    }

    [Fact]
    public void ComponentWithInlineString_MultipleEntities_AreIndependent()
    {
        var world = WorldFactory.Shared.CreateWorld();
        world.InitComponent<BlittableString32Component>();

        var e1 = world.CreateEntityId();
        var e2 = world.CreateEntityId();

        ref var c1 = ref world.AddComponent<BlittableString32Component>(e1);
        ref var c2 = ref world.AddComponent<BlittableString32Component>(e2);

        BlittableString32.TryCreate("entity_a", out c1.Name);
        BlittableString32.TryCreate("entity_b", out c2.Name);

        world.GetComponent<BlittableString32Component>(e1).Name.ToString().Should().Be("entity_a");
        world.GetComponent<BlittableString32Component>(e2).Name.ToString().Should().Be("entity_b");
    }
}
